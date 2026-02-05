using System.Threading.Tasks;

namespace QueueManagementSystem.Application.Interfaces
{
    public interface IQueueNotifier
    {
        Task NotifyQueueUpdatedAsync(int serviceId, int branchId);
        Task NotifyTicketUpdatedAsync(int ticketId, int serviceId, int branchId);
    }
}
