using System.Threading.Tasks;

namespace QueueManagementSystem.Application.Interfaces
{
    public interface ISmsNotifier
    {
        Task SendTicketCreatedAsync(string phoneNumber, int ticketNumber, string serviceName, int estimatedWaitMinutes);
        Task SendTicketCalledAsync(string phoneNumber, int ticketNumber, string serviceName);
    }
}
