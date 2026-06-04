using Microsoft.AspNetCore.Identity;
using TechMove.Database.DTOs;
using TechMove.Database.Models;
using Microsoft.EntityFrameworkCore;
using TechMove.Backend_API.Data;

namespace TechMove.Backend_API.Services
{
    public class AdminService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminService(IDbContextFactory<AppDbContext> contextFactory, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list all users
        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var listOfUsers = new List<AdminUserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                listOfUsers.Add(new AdminUserDto { Id = user.Id, Email = user.Email ?? string.Empty, Role = roles.FirstOrDefault() ?? "No Role" });
            }

            return listOfUsers;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method creates new users
        public async Task<IdentityResult> CreateUserAsync(string email, string password, string? role = null)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            
            if (!result.Succeeded)
            {
                return result;
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);

                if (role.Equals("Client", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using var context = await _contextFactory.CreateDbContextAsync();

                        var newClientProfile = new Client
                        {
                            UserId = user.Id,
                            clientName = email,
                            clientEmail = email,
                            clientPhoneNumber = "000-000-0000"
                        };

                        context.Clients.Add(newClientProfile);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return IdentityResult.Failed(new IdentityError
                        {
                            Description = $"User account created, but Client profile initialization failed: {ex.Message}"
                        });
                    }
                }
            }
            return result;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method deletes existing users
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                var linkedClientProfile = await context.Clients
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (linkedClientProfile != null)
                {
                    context.Clients.Remove(linkedClientProfile);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return IdentityResult.Failed(new IdentityError
                {
                    Description = $"Failed to delete the user: {innerMessage}"
                });
            }

            return await _userManager.DeleteAsync(user);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method changes the role of an existing user
        public async Task<IdentityResult> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            
            if (!removeResult.Succeeded)
            {
                return removeResult;
            }

            if (!await _roleManager.RoleExistsAsync(newRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(newRole));
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            return addResult;
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
