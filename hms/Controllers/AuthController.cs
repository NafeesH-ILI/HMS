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
    [Route("/api/v2/auth")]
    public class AuthController(
        ILogger<AuthController> logger,
        IUserService userService,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        DbCtx ctx) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _users = userManager;
        private readonly DbCtx _ctx = ctx;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto auth)
        {
            Microsoft.AspNetCore.Identity.SignInResult res =
                await _signInManager.PasswordSignInAsync(auth.UName,
                    auth.Password, false, false);
            if (!res.Succeeded)
            {
                _logger.LogError(res.ToString());
                return Unauthorized("bad credentials");
            }
            return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("password-reset/request")]
        public async Task<PasswordResetSessionDto> PasswordReset([FromBody] PassResetRequestDto req)
        {
            PassResetOtp otp = await _userService.PasswordReset(req.UName);
            _logger.LogError("Email sending not implemented, OTP for {uname}:{Id} is {Otp}", req.UName, otp.Id, otp.Otp);
            return new PasswordResetSessionDto { SessionId = otp.Id };
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordResetOtp([FromBody] PasswordResetDto password)
        {
            await _userService.PasswordReset(password);
            return Ok();
        }

        [HttpPost("password/{uname}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PasswordResetByAdmin(string uname, [FromBody] PasswordDto password)
        {
            await _userService.PasswordChange(uname, password.Password);
            return Ok();
        }

        [HttpPost("password")]
        [Authorize]
        public async Task<IActionResult> PasswordChange([FromBody] PasswordChangeDto req)
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrForbidden();
            var res = await _users.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
            if (res.Succeeded)
                return Ok();
            return BadRequest(res.Errors);
        }
    }
}
