using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Infrastructure.Persistence
{
    public class QueueDbContext: IdentityDbContext<ApplicationUser>
    {
        public QueueDbContext(DbContextOptions<QueueDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<TicketHistoryLog> TicketHistoryLogs { get; set; }
        // Define DbSet properties for your entities
        // public DbSet<YourEntity> YourEntities { get; set; }

        // Example:
        // public DbSet<Customer> Customers { get; set; }
        // public DbSet<Ticket> Tickets { get; set; }
        // public DbSet<Queue> Queues { get; set; }
    }
}
