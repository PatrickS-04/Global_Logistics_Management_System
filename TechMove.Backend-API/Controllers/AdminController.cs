using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TechMove.Backend_API.Services;
using TechMove.Database.Models;

namespace TechMove.Backend_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method returns a list of all users
        [HttpGet("getUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allows admins to create a user
        [HttpPost("createUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromForm] string email, [FromForm] string password, [FromForm] string role)
        {
            var result = await _adminService.CreateUserAsync(email, password, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { Message = "User was created Successfully!" });
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allows admins to delete a user
        [HttpDelete("deleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromForm] string userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { Message = "User was deleted Successfully!" });
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allows admins to change the role of the user
        [HttpPost("changeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole([FromForm] string userId, [FromForm] string newRole)
        {
            var result = await _adminService.ChangeUserRoleAsync(userId, newRole);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { Message = "Users role was successfully changed" });
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
