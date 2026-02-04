using Microsoft.AspNetCore.SignalR;
using QueueManagementSystem.API.Hubs;
using QueueManagementSystem.Application.Interfaces;

namespace QueueManagementSystem.API.Services
{
    public class SignalRQueueNotifier : IQueueNotifier
    {
        private readonly IHubContext<QueueHub> _hubContext;

        public SignalRQueueNotifier(IHubContext<QueueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyQueueUpdatedAsync(int serviceId, int branchId)
        {
            string groupName = QueueHub.BuildGroupName(serviceId, branchId);
            return _hubContext.Clients.Group(groupName).SendAsync("QueueUpdated", serviceId, branchId);
        }

        public Task NotifyTicketUpdatedAsync(int ticketId, int serviceId, int branchId)
        {
            string groupName = QueueHub.BuildGroupName(serviceId, branchId);
            return _hubContext.Clients.Group(groupName).SendAsync("TicketUpdated", ticketId, serviceId, branchId);
        }
    }
}
