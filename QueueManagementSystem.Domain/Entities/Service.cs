using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public int AvgServiceTime { get; set; }
        public bool IsOpen { get; set; }
        public int? BranchId { get; set; }

        // Navigation
        public Branch Branch { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
