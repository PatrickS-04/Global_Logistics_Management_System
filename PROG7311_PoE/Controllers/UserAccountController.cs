using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG7311_PoE.Components.Pages;

namespace PROG7311_PoE.Controllers
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

        
        [HttpPost("logout")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Login");
        }
    }
}
