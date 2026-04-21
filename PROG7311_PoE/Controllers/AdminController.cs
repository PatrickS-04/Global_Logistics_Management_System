using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PROG7311_PoE.Services;

namespace PROG7311_PoE.Controllers
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

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromForm] string email, [FromForm] string password, [FromForm] string role)
        {
            var result = await _adminService.CreateUserAsync(email, password, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Redirect("/manage-users");
        }

        [HttpPost("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromForm] string userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Redirect("/manage-users");
        }

        [HttpPost("changeRole")]
        public async Task<IActionResult> ChangeRole([FromForm] string userId, [FromForm] string newRole)
        {
            var result = await _adminService.ChangeUserRoleAsync(userId, newRole);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Redirect("/manage-users");
        }
    }
}
