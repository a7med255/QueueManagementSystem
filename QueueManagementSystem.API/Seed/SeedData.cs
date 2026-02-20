using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.Domain.Entities;
using QueueManagementSystem.Infrastructure.Persistence;

namespace QueueManagementSystem.API.Seed
{
    public static class SeedData
    {
        private const string AdminRole = "Admin";
        private const string CustomerRole = "Customer";
        private const string AdminEmail = "admin@queue.local";
        private const string AdminPassword = "Admin@12345";

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            QueueDbContext dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

            await dbContext.Database.MigrateAsync();

            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            }

            if (!await roleManager.RoleExistsAsync(CustomerRole))
            {
                await roleManager.CreateAsync(new IdentityRole(CustomerRole));
            }

            ApplicationUser adminUser = await userManager.FindByEmailAsync(AdminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    Name = "System Admin",
                    CreatedDate = DateTime.UtcNow
                };

                IdentityResult createResult = await userManager.CreateAsync(adminUser, AdminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AdminRole);
                }
            }

            Branch mainBranch = await EnsureBranchAsync(dbContext, "Main Branch", "Downtown");
            Branch northBranch = await EnsureBranchAsync(dbContext, "North Branch", "Uptown");

            if (mainBranch == null)
            {
                logger.LogWarning("Seed data skipped because no branches could be created or loaded.");
                return;
            }

            await EnsureServiceAsync(dbContext, mainBranch.Id, "General Inquiry", 5);
            await EnsureServiceAsync(dbContext, mainBranch.Id, "Examination", 15);

            await EnsureCounterAsync(dbContext, mainBranch.Id, "Counter A");
            await EnsureCounterAsync(dbContext, mainBranch.Id, "Counter B");
        }

        private static async Task<Branch> EnsureBranchAsync(QueueDbContext dbContext, string branchName, string location)
        {
            Branch branch = await dbContext.Branches.FirstOrDefaultAsync(current => current.BranchName == branchName);
            if (branch != null)
            {
                return branch;
            }

            branch = new Branch { BranchName = branchName, Location = location };
            dbContext.Branches.Add(branch);
            await dbContext.SaveChangesAsync();
            return branch;
        }

        private static async Task EnsureServiceAsync(QueueDbContext dbContext, int branchId, string serviceName, int avgServiceTime)
        {
            bool exists = await dbContext.Services.AnyAsync(service =>
                service.BranchId == branchId && service.ServiceName == serviceName);
            if (exists)
            {
                return;
            }

            dbContext.Services.Add(new Service
            {
                ServiceName = serviceName,
                AvgServiceTime = avgServiceTime,
                IsOpen = true,
                BranchId = branchId
            });
            await dbContext.SaveChangesAsync();
        }

        private static async Task EnsureCounterAsync(QueueDbContext dbContext, int branchId, string counterName)
        {
            bool exists = await dbContext.Counters.AnyAsync(counter =>
                counter.BranchId == branchId && counter.CounterName == counterName);
            if (exists)
            {
                return;
            }

            dbContext.Counters.Add(new Counter
            {
                CounterName = counterName,
                BranchId = branchId,
                IsOpen = true
            });
            await dbContext.SaveChangesAsync();
        }
    }
}
