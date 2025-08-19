using hms.Utils;
using hms.Models;
using hms.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ErrorHandler]
    [Route("/api/v2/auth")]
    public class AuthController(
        ILogger<AuthController> logger,
        SignInManager<User> signInManager,
        UserManager<User> userManager) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly UserManager<User> _users = userManager;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto auth)
        {
            Microsoft.AspNetCore.Identity.SignInResult res =
                await _signInManager.PasswordSignInAsync(auth.UName,
                    auth.Password, false, false);
            if (!res.Succeeded)
                return Unauthorized("bad credentials");
            return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("whoami")]
        [Authorize]
        public async Task<WhoAmI> WhoAmI()
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrNotFound();
            IList<string> roles = await _users.GetRolesAsync(user);
            return new WhoAmI
            {
                Role = roles.FirstOrDefault() ?? "NULL",
                UName = user.UserName ?? "NULL"
            };
        }
    }
}
