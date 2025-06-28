using QueueManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Domain.Entities
{
    public class TicketHistoryLog
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public TicketStatus StatusFrom { get; set; }
        public TicketStatus StatusTo { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedByUserId { get; set; }

        // Navigation
        public Ticket Ticket { get; set; }
    }
}
