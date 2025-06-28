using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string Location { get; set; }

        // Navigation
        public ICollection<Counter> Counters { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
