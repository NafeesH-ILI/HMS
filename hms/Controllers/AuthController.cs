using AutoMapper;
using hms.Common;
using hms.Models;
using hms.Models.DTOs;
using hms.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace hms.Controllers
{
    [ErrorHandler]
    [Route("/api/v2/auth")]
    public class AuthController(
        ILogger<AuthController> logger,
        IMapper mapper,
        IUserService userService) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto auth)
        {
            if (!await _userService.Authenticate(auth.UName, auth.Password))
                return Unauthorized("bad credentials");
            User user = await _userService.GetByUName(auth.UName);
            List<Claim> claims =
            [
                new (ClaimTypes.Name, auth.UName),
                new (ClaimTypes.Role, user.Type.ToString())
            ];
            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties props = new();
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), props);
            _logger.LogInformation("User {UName} Logged in", user.UName);
            return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("password-reset/{uname}")]
        public async Task<IActionResult> passReset(string uname)
        {

            return Ok();
        }

        [HttpGet("whoami")]
        [Authorize]
        public WhoAmI WhoAmI()
        {
            return new WhoAmI
            {
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                UName = User.Identity?.Name
            };
        }
    }
}
