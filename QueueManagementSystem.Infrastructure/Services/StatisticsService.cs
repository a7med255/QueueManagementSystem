using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.Application.DTOs.Statistics;
using QueueManagementSystem.Application.Interfaces;
using QueueManagementSystem.Domain.Enums;
using QueueManagementSystem.Infrastructure.Persistence;

namespace QueueManagementSystem.Infrastructure.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly QueueDbContext _context;

        public StatisticsService(QueueDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceStatisticsDto> GetServiceStatisticsAsync(int serviceId, int branchId)
        {
            var tickets = await _context.Tickets
                .Where(ticket => ticket.ServiceId == serviceId && ticket.BranchId == branchId)
                .ToListAsync();

            var doneTickets = tickets.Where(ticket => ticket.Status == TicketStatus.Done).ToList();
            double averageWait = doneTickets.Count == 0
                ? 0
                : doneTickets.Average(ticket => (ticket.StartedAt ?? ticket.CreatedAt) - ticket.CreatedAt)
                    .TotalMinutes;


            var peakHour = tickets
                .GroupBy(ticket => ticket.CreatedAt.Hour)
                .OrderByDescending(group => group.Count())
                .Select(group => (int?)group.Key)
                .FirstOrDefault();

            var serviceName = await _context.Services
                .Where(service => service.Id == serviceId)
                .Select(service => service.ServiceName)
                .FirstOrDefaultAsync();

            return new ServiceStatisticsDto
            {
                ServiceId = serviceId,
                ServiceName = serviceName,
                TotalCustomersServed = doneTickets.Count,
                AverageWaitingTimeMinutes = Math.Round(averageWait, 2),
                PeakHourStartUtc = peakHour.HasValue ? TimeSpan.FromHours(peakHour.Value) : null,
                PeakHourEndUtc = peakHour.HasValue ? TimeSpan.FromHours(peakHour.Value + 1) : null
            };
        }
    }
}
