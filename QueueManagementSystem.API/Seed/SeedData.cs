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

            if (!await dbContext.Branches.AnyAsync())
            {
                dbContext.Branches.AddRange(
                    new Branch { BranchName = "Main Branch", Location = "Downtown" },
                    new Branch { BranchName = "North Branch", Location = "Uptown" }
                );
                await dbContext.SaveChangesAsync();
            }

            if (!await dbContext.Services.AnyAsync())
            {
                List<Branch> branches = await dbContext.Branches.ToListAsync();
                Branch mainBranch = branches.First();

                dbContext.Services.AddRange(
                    new Service { ServiceName = "General Inquiry", AvgServiceTime = 5, IsOpen = true, BranchId = mainBranch.Id },
                    new Service { ServiceName = "Examination", AvgServiceTime = 15, IsOpen = true, BranchId = mainBranch.Id }
                );
                await dbContext.SaveChangesAsync();
            }

            if (!await dbContext.Counters.AnyAsync())
            {
                List<Branch> branches = await dbContext.Branches.ToListAsync();
                Branch mainBranch = branches.First();

                dbContext.Counters.AddRange(
                    new Counter { CounterName = "Counter A", BranchId = mainBranch.Id, IsOpen = true },
                    new Counter { CounterName = "Counter B", BranchId = mainBranch.Id, IsOpen = true }
                );
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
