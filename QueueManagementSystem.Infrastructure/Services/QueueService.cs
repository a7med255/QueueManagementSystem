using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.Application.DTOs.Tickets;
using QueueManagementSystem.Application.Interfaces;
using QueueManagementSystem.Domain.Entities;
using QueueManagementSystem.Domain.Enums;
using QueueManagementSystem.Infrastructure.Persistence;

namespace QueueManagementSystem.Infrastructure.Services
{
    public class QueueService : IQueueService
    {
        private readonly QueueDbContext _context;
        private readonly IQueueNotifier _notifier;
        private readonly ISmsNotifier _smsNotifier;

        public QueueService(QueueDbContext context, IQueueNotifier notifier, ISmsNotifier smsNotifier)
        {
            _context = context;
            _notifier = notifier;
            _smsNotifier = smsNotifier;
        }

        public async Task<TicketDto> CreateTicketAsync(string customerId, CreateTicketRequestDto request)
        {
            Service service = await _context.Services.FirstOrDefaultAsync(current => current.Id == request.ServiceId);
            if (service == null || !service.IsOpen)
            {
                return null;
            }

            int nextNumber = await _context.Tickets
                .Where(ticket => ticket.ServiceId == request.ServiceId && ticket.BranchId == request.BranchId)
                .Select(ticket => ticket.TicketNumber)
                .DefaultIfEmpty(0)
                .MaxAsync() + 1;

            int waitingCount = await _context.Tickets.CountAsync(ticket =>
                ticket.ServiceId == request.ServiceId &&
                ticket.BranchId == request.BranchId &&
                ticket.Status == TicketStatus.Waiting);

            Ticket ticket = new Ticket
            {
                TicketNumber = nextNumber,
                ServiceId = request.ServiceId,
                BranchId = request.BranchId,
                Status = TicketStatus.Waiting,
                CreatedAt = DateTime.UtcNow,
                EstimatedWaitTime = waitingCount * service.AvgServiceTime,
                CustomerId = customerId
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await _notifier.NotifyQueueUpdatedAsync(request.ServiceId, request.BranchId);
            await _notifier.NotifyTicketUpdatedAsync(ticket.Id, request.ServiceId, request.BranchId);
            await SendTicketCreatedNotificationAsync(request.PhoneNumber, customerId, ticket, service.ServiceName);

            return await MapTicketAsync(ticket.Id);
        }

        public async Task<QueueStatusDto> GetQueueAsync(int serviceId, int branchId)
        {
            Service service = await _context.Services
                .Include(current => current.Branch)
                .FirstOrDefaultAsync(current => current.Id == serviceId);

            if (service == null)
            {
                return null;
            }

            List<TicketDto> tickets = await _context.Tickets
                .Include(ticket => ticket.Counter)
                .Where(ticket => ticket.ServiceId == serviceId && ticket.BranchId == branchId)
                .OrderBy(ticket => ticket.CreatedAt)
                .Select(ticket => new TicketDto
                {
                    Id = ticket.Id,
                    TicketNumber = ticket.TicketNumber,
                    ServiceId = ticket.ServiceId,
                    ServiceName = service.ServiceName,
                    BranchId = ticket.BranchId,
                    BranchName = service.Branch != null ? service.Branch.BranchName : null,
                    CounterId = ticket.CounterId,
                    CounterName = ticket.Counter != null ? ticket.Counter.CounterName : null,
                    Status = ticket.Status,
                    CreatedAt = ticket.CreatedAt,
                    StartedAt = ticket.StartedAt,
                    EndedAt = ticket.EndedAt,
                    EstimatedWaitTime = ticket.EstimatedWaitTime,
                    PositionInQueue = 0
                })
                .ToListAsync();

            List<int> waitingTicketIds = tickets
                .Where(ticket => ticket.Status == TicketStatus.Waiting)
                .OrderBy(ticket => ticket.CreatedAt)
                .Select(ticket => ticket.Id)
                .ToList();

            foreach (TicketDto ticket in tickets)
            {
                if (ticket.Status == TicketStatus.Waiting)
                {
                    ticket.PositionInQueue = waitingTicketIds.IndexOf(ticket.Id) + 1;
                }
            }

            return new QueueStatusDto
            {
                ServiceId = serviceId,
                ServiceName = service.ServiceName,
                BranchId = branchId,
                BranchName = service.Branch?.BranchName,
                WaitingCount = tickets.Count(ticket => ticket.Status == TicketStatus.Waiting),
                InServiceCount = tickets.Count(ticket => ticket.Status == TicketStatus.InService),
                Tickets = tickets
            };
        }

        public async Task<TicketDto> CallNextAsync(TicketActionRequestDto request, string agentUserId)
        {
            Ticket ticket = await _context.Tickets
                .Where(current =>
                    current.ServiceId == request.ServiceId &&
                    current.BranchId == request.BranchId &&
                    current.Status == TicketStatus.Waiting)
                .OrderBy(current => current.CreatedAt)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return null;
            }

            ticket.Status = TicketStatus.InService;
            ticket.StartedAt = DateTime.UtcNow;
            ticket.CounterId = request.CounterId;

            _context.TicketHistoryLogs.Add(new TicketHistoryLog
            {
                TicketId = ticket.Id,
                StatusFrom = TicketStatus.Waiting,
                StatusTo = TicketStatus.InService,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = agentUserId
            });

            await _context.SaveChangesAsync();
            await _notifier.NotifyQueueUpdatedAsync(request.ServiceId, request.BranchId);
            await _notifier.NotifyTicketUpdatedAsync(ticket.Id, request.ServiceId, request.BranchId);
            await SendTicketCalledNotificationAsync(ticket, ticket.ServiceId, ticket.BranchId);

            return await MapTicketAsync(ticket.Id);
        }

        public async Task<TicketDto> SkipAsync(int ticketId, string agentUserId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(current => current.Id == ticketId);
            if (ticket == null)
            {
                return null;
            }

            TicketStatus previousStatus = ticket.Status;
            ticket.Status = TicketStatus.Waiting;
            ticket.StartedAt = null;
            ticket.CounterId = null;

            _context.TicketHistoryLogs.Add(new TicketHistoryLog
            {
                TicketId = ticket.Id,
                StatusFrom = previousStatus,
                StatusTo = TicketStatus.Waiting,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = agentUserId
            });

            await _context.SaveChangesAsync();
            await _notifier.NotifyQueueUpdatedAsync(ticket.ServiceId, ticket.BranchId);
            await _notifier.NotifyTicketUpdatedAsync(ticket.Id, ticket.ServiceId, ticket.BranchId);

            return await MapTicketAsync(ticket.Id);
        }

        public async Task<TicketDto> CancelAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(current => current.Id == ticketId);
            if (ticket == null)
            {
                return null;
            }

            TicketStatus previousStatus = ticket.Status;
            ticket.Status = TicketStatus.Canceled;
            ticket.EndedAt = DateTime.UtcNow;

            _context.TicketHistoryLogs.Add(new TicketHistoryLog
            {
                TicketId = ticket.Id,
                StatusFrom = previousStatus,
                StatusTo = TicketStatus.Canceled,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = userId
            });

            await _context.SaveChangesAsync();
            await _notifier.NotifyQueueUpdatedAsync(ticket.ServiceId, ticket.BranchId);
            await _notifier.NotifyTicketUpdatedAsync(ticket.Id, ticket.ServiceId, ticket.BranchId);

            return await MapTicketAsync(ticket.Id);
        }

        private async Task<TicketDto> MapTicketAsync(int ticketId)
        {
            Ticket ticket = await _context.Tickets
                .Include(current => current.Service)
                .Include(current => current.Branch)
                .Include(current => current.Counter)
                .FirstOrDefaultAsync(current => current.Id == ticketId);

            if (ticket == null)
            {
                return null;
            }

            int position = 0;
            if (ticket.Status == TicketStatus.Waiting)
            {
                List<int> waitingTicketIds = await _context.Tickets
                    .Where(current =>
                        current.ServiceId == ticket.ServiceId &&
                        current.BranchId == ticket.BranchId &&
                        current.Status == TicketStatus.Waiting)
                    .OrderBy(current => current.CreatedAt)
                    .Select(current => current.Id)
                    .ToListAsync();

                position = waitingTicketIds.IndexOf(ticket.Id) + 1;
            }

            return new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                ServiceId = ticket.ServiceId,
                ServiceName = ticket.Service?.ServiceName,
                BranchId = ticket.BranchId,
                BranchName = ticket.Branch?.BranchName,
                CounterId = ticket.CounterId,
                CounterName = ticket.Counter?.CounterName,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                StartedAt = ticket.StartedAt,
                EndedAt = ticket.EndedAt,
                EstimatedWaitTime = ticket.EstimatedWaitTime,
                PositionInQueue = position
            };
        }

        private async Task SendTicketCreatedNotificationAsync(string phoneNumber, string customerId, Ticket ticket, string serviceName)
        {
            string resolvedPhone = phoneNumber;
            if (string.IsNullOrWhiteSpace(resolvedPhone) && !string.IsNullOrWhiteSpace(customerId))
            {
                resolvedPhone = await _context.Users
                    .Where(user => user.Id == customerId)
                    .Select(user => user.PhoneNumber)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrWhiteSpace(resolvedPhone))
            {
                return;
            }

            await _smsNotifier.SendTicketCreatedAsync(resolvedPhone, ticket.TicketNumber, serviceName, ticket.EstimatedWaitTime);
        }

        private async Task SendTicketCalledNotificationAsync(Ticket ticket, int serviceId, int branchId)
        {
            if (string.IsNullOrWhiteSpace(ticket.CustomerId))
            {
                return;
            }

            string phoneNumber = await _context.Users
                .Where(user => user.Id == ticket.CustomerId)
                .Select(user => user.PhoneNumber)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return;
            }

            string serviceName = await _context.Services
                .Where(service => service.Id == serviceId)
                .Select(service => service.ServiceName)
                .FirstOrDefaultAsync();

            await _smsNotifier.SendTicketCalledAsync(phoneNumber, ticket.TicketNumber, serviceName ?? "Service");
        }
    }
}
