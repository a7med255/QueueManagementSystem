using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Domain.Entities
{
    public class Counter
    {
        public int Id { get; set; }
        public string CounterName { get; set; }
        public int BranchId { get; set; }
        public bool IsOpen { get; set; }
        public int? CurrentTicketId { get; set; }

        // Navigation
        public Branch Branch { get; set; }
        [NotMapped]
        public Ticket CurrentTicket { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
