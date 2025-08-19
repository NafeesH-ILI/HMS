using hms.Utils;
using hms.Models;
using hms.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using hms.Repos.Interfaces;
using hms.Services;
using hms.Services.Interfaces;
=======
>>>>>>> master

namespace hms.Controllers
{
    [ErrorHandler]
    [Route("/api/v2/auth")]
    public class AuthController(
        ILogger<AuthController> logger,
<<<<<<< HEAD
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
=======
        SignInManager<User> signInManager,
        UserManager<User> userManager) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly UserManager<User> _users = userManager;
>>>>>>> master

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

<<<<<<< HEAD
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
=======
        [HttpGet("whoami")]
        [Authorize]
        public async Task<WhoAmI> WhoAmI()
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrNotFound();
            IList<string> roles = await _users.GetRolesAsync(user);
            return new WhoAmI
            {
                Role = roles.FirstOrDefault() ?? "NULL",
                UName = user.UserName
            };
>>>>>>> master
        }
    }
}
