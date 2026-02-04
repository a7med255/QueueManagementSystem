using System.Collections.Generic;

namespace QueueManagementSystem.Application.DTOs.Tickets
{
    public class QueueStatusDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int WaitingCount { get; set; }
        public int InServiceCount { get; set; }
        public IReadOnlyCollection<TicketDto> Tickets { get; set; }
    }
}
