using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PROG7311_PoE.Models;

namespace PROG7311_PoE.Data
{
    public class InitializeDatabase
    {
        public static async Task TestUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

            string[] roleNames = { "Admin", "LogisticsManager", "Client" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            await CreateUserIfDoesNotExist(userManager, "admin@test.com", "Admin@123", "Admin");
            await CreateUserIfDoesNotExist(userManager, "manager@test.com", "Manager@123", "LogisticsManager");
            await CreateUserIfDoesNotExist(userManager, "client@test.com", "Client@123", "Client");

            var client = await userManager.FindByEmailAsync("client@test.com");
            if (client != null)
            {
                using var db = contextFactory.CreateDbContext();
                if (!db.Clients.Any(c => c.UserId == client.Id))
                {
                    db.Clients.Add(new Client
                    {
                        UserId = client.Id,
                        clientName = "Test Client",
                        clientEmail = "client@test.com",
                        clientPhoneNumber = "0123456789"
                    });
                    await db.SaveChangesAsync();
                }
            }
        }

        public static async Task CreateUserIfDoesNotExist(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
