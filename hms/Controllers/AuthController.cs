using hms.Utils;
using hms.Models;
using hms.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using hms.Repos.Interfaces;
using hms.Services;
using hms.Services.Interfaces;

namespace hms.Controllers
{
    [ErrorHandler]
    [Route("/api/v2/auth")]
    public class AuthController(
        ILogger<AuthController> logger,
        //IPassResetService passResetService,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        DbCtx ctx) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly SignInManager<User> _signInManager = signInManager;
        //private readonly IPassResetService _passResetService = passResetService;
        private readonly UserManager<User> _users = userManager;
        private readonly DbCtx _ctx = ctx;

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

        [HttpPost("password-reset")]
        //public async Task<PasswordResetSessionDto> PasswordReset([FromBody] string uname)
        public async Task PasswordReset([FromBody] string uname)
        {
            /*PassResetOtp otp = await _passResetService.New(uname);
            _logger.LogError("Email sending not implemented, OTP for {uname}:{Id} is {Otp}", uname, otp.Id, otp.Otp);
            return new PasswordResetSessionDto { SessionId = otp.Id };*/
            string token = await _users.GeneratePasswordResetTokenAsync(
                await _users.GetUserAsync(User) ?? throw new ErrUnauthorized());
            _logger.LogError("Email sending not implemented, Token for {uname} is {Otp}", uname, token);
        }

        [HttpPost("password-reset/otp")]
        public async Task<IActionResult> PasswordResetOtp([FromBody] PasswordDto password, [FromQuery] string token)
        {
            await _users.ResetPasswordAsync(
                await _users.GetUserAsync(User) ?? throw new ErrUnauthorized(),
                token, password.Password);
            return Ok();
        }

        [HttpPost("password/{uname}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PasswordResetByAdmin(string uname, [FromBody] PasswordDto password)
        {
            User user = await _users.FindByNameAsync(uname) ?? throw new ErrNotFound();
            string token = await _users.GeneratePasswordResetTokenAsync(user);
            var res = await _users.ResetPasswordAsync(user, token, password.Password);
            if (!res.Succeeded)
                throw new Exception(res.Errors.ToString());
            await _ctx.SaveChangesAsync();
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
