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

        private async Task<PaginatedResponse<IEnumerable<UserDtoGet>>> GetAllByType(hms.Models.User.Types type,
            int page = 1, int pageSize = 10)
        {
            IEnumerable<UserDtoGet> users = from u in await _userService.GetByType(type, page, pageSize)
                                            select _mapper.Map<UserDtoGet>(u);
            int count = await _userService.CountByType(type);
            return new PaginatedResponse<IEnumerable<UserDtoGet>>{
                Count = count,
                Value = users
            };
        }

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

        [HttpGet("users/{uname}", Name="GetByUName")]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<UserDtoGet> Get(string uname)
        {
            User user = await _userService.GetByUName(uname) ?? throw new ErrNotFound();
            return _mapper.Map<UserDtoGet>(user);
        }

        [HttpPatch("users/{uname}")]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<IActionResult> Patch(string uname, [FromBody] UserDtoUpdate userUpdated)
        {
            string actorUName = User.Identity?.Name ?? throw new ErrUnauthorized();
            await _userService.UpdatePassword(actorUName, uname, userUpdated.Password);
            return Ok();
        }

        [HttpPost("users")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] UserDtoNew userNew)
        {
            string actorUName = User.Identity?.Name ?? throw new ErrUnauthorized();
            UserDtoGet dto = _mapper.Map<UserDtoGet>(await _userService.Add(actorUName, userNew));
            return CreatedAtRoute("GetByUName", new { uname = dto.UName }, dto);
        }

        [HttpGet("users")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<PaginatedResponse<IEnumerable<UserDtoGet>>> GetAll(int page = 1, int page_size = 10,
            string? type = null)
        {
            if (type == null)
            {
                IEnumerable<UserDtoGet> users = from u in await _userService.Get(page, page_size)
                                                select _mapper.Map<UserDtoGet>(u);
                int count = await _userService.Count();
                return new PaginatedResponse<IEnumerable<UserDtoGet>>
                {
                    Count = count,
                    Value = users
                };
            }
            return type switch
            {
                "superadmins" => await GetAllByType(hms.Models.User.Types.SuperAdmin, page, page_size),
                "admins" => await GetAllByType(hms.Models.User.Types.Admin, page, page_size),
                "receptionists" => await GetAllByType(hms.Models.User.Types.Receptionist, page, page_size),
                "doctors" => await GetAllByType(hms.Models.User.Types.Doctor, page, page_size),
                "patients" => await GetAllByType(hms.Models.User.Types.Patient, page, page_size),
                _ => throw new ErrBadReq(),
            };
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
