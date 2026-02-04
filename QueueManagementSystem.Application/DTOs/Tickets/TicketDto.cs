using System;
using QueueManagementSystem.Domain.Enums;

namespace QueueManagementSystem.Application.DTOs.Tickets
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int TicketNumber { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int? CounterId { get; set; }
        public string CounterName { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int EstimatedWaitTime { get; set; }
        public int PositionInQueue { get; set; }
    }
}
