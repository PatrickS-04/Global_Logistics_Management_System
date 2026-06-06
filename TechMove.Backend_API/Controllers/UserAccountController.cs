using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechMove.Backend_API.Controllers
{
    [Route("api/account")]
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User was NOT logged in successfully");
            }

            var userLoggedIn = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure : false);

            if (userLoggedIn.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Email),
                };

                var roles = await _signInManager.UserManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

                return Ok(new { Message = "User Successfully Logged In!" });
            }

            return BadRequest("User was NOT logged in successfully");
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method logs users out
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "User was successfully Logged Out" });
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets the users log in state whether they are logged in or logged out
        [HttpGet("userState")]
        public IActionResult GetUserState()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var roles = User.FindAll(ClaimTypes.Role).Select(currentUser => currentUser.Value).ToList();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return Ok(new
                {
                    isAuthenticated = true,
                    id = userId,
                    name = User.Identity.Name,
                    roles = roles
                });
            }

            return Ok(new { isAuthenticated = false });
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
