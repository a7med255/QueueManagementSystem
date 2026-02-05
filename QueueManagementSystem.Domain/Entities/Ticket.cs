using QueueManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int TicketNumber { get; set; }
        public int ServiceId { get; set; }
        public int BranchId { get; set; }
        public int? CounterId { get; set; }
        public string? CustomerId { get; set; }

        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int EstimatedWaitTime { get; set; }

        // Navigation
        public Service Service { get; set; }
        public Branch Branch { get; set; }
        public Counter Counter { get; set; }
        public ApplicationUser? Customer { get; set; }
    }
}
