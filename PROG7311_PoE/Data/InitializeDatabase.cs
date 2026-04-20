using Microsoft.AspNetCore.Identity;

namespace PROG7311_PoE.Data
{
    public class InitializeDatabase
    {
        public static async Task TestUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

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
