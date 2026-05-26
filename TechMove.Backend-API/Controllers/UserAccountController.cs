using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechMove.Database.Models;
using TechMove.Backend_API.Services;

namespace TechMove.Backend_API.Controllers
{
    [Route("login-action")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserAccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method logs in users with correct credentials
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            var userLoggedIn = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure : false);

            if (userLoggedIn.Succeeded)
            {
                return Redirect("/Home");
            }

            return Redirect("/Login");
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method logs users out
        [HttpPost("logout")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Login");
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
