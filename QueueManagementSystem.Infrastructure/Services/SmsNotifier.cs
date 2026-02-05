using Microsoft.Extensions.Logging;
using QueueManagementSystem.Application.Interfaces;

namespace QueueManagementSystem.Infrastructure.Services
{
    public class SmsNotifier : ISmsNotifier
    {
        private readonly ILogger<SmsNotifier> _logger;

        public SmsNotifier(ILogger<SmsNotifier> logger)
        {
            _logger = logger;
        }

        public Task SendTicketCreatedAsync(string phoneNumber, int ticketNumber, string serviceName, int estimatedWaitMinutes)
        {
            _logger.LogInformation(
                "SMS to {PhoneNumber}: Ticket {TicketNumber} created for {ServiceName}. Estimated wait {EstimatedWaitMinutes} minutes.",
                phoneNumber,
                ticketNumber,
                serviceName,
                estimatedWaitMinutes);

            return Task.CompletedTask;
        }

        public Task SendTicketCalledAsync(string phoneNumber, int ticketNumber, string serviceName)
        {
            _logger.LogInformation(
                "SMS to {PhoneNumber}: Ticket {TicketNumber} for {ServiceName} is now called. Please proceed to the counter.",
                phoneNumber,
                ticketNumber,
                serviceName);

            return Task.CompletedTask;
        }
    }
}
