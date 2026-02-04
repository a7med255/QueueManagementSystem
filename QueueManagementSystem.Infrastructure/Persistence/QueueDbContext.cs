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

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(branch => branch.BranchName)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(branch => branch.Location)
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(service => service.ServiceName)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.HasIndex(service => new { service.BranchId, service.ServiceName })
                    .IsUnique();

                entity.Property(service => service.IsOpen)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.Property(counter => counter.CounterName)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(ticket => ticket.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(ticket => ticket.Service)
                    .WithMany(service => service.Tickets)
                    .HasForeignKey(ticket => ticket.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ticket => ticket.Branch)
                    .WithMany(branch => branch.Tickets)
                    .HasForeignKey(ticket => ticket.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ticket => ticket.Counter)
                    .WithMany(counter => counter.Tickets)
                    .HasForeignKey(ticket => ticket.CounterId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(ticket => ticket.Customer)
                    .WithMany()
                    .HasForeignKey(ticket => ticket.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TicketHistoryLog>(entity =>
            {
                entity.Property(history => history.ChangedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(history => history.Ticket)
                    .WithMany()
                    .HasForeignKey(history => history.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
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
