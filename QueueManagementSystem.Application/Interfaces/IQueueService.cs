using System.Collections.Generic;
using System.Threading.Tasks;
using QueueManagementSystem.Application.DTOs.Tickets;

namespace QueueManagementSystem.Application.Interfaces
{
    public interface IQueueService
    {
        Task<TicketDto> CreateTicketAsync(string customerId, CreateTicketRequestDto request);
        Task<QueueStatusDto> GetQueueAsync(int serviceId, int branchId);
        Task<TicketDto> CallNextAsync(TicketActionRequestDto request, string agentUserId);
        Task<TicketDto> SkipAsync(int ticketId, string agentUserId);
        Task<TicketDto> CancelAsync(int ticketId, string userId);
    }
}
