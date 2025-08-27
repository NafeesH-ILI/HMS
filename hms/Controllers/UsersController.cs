using AutoMapper;
using hms.Models;
using hms.Models.DTOs;
using hms.Services.Interfaces;
using hms.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/users")]
    [Authorize]
    public class UsersController(
        IUserService userService,
        IDoctorService doctorService,
        IPatientService patientService,
        UserManager<User> users,
        IMapper mapper) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _users = users;
        private readonly IDoctorService _doctors = doctorService;
        private readonly IPatientService _patients = patientService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public PaginatedResponse<IEnumerable<UserDtoGet>> GetAll(int page = 1, int page_size = 10, string? type = null)
        {
            if (type == null)
            {
                return new PaginatedResponse<IEnumerable<UserDtoGet>>
                {
                    Value = from u in _userService.Get(page, page_size) select _mapper.Map<UserDtoGet>(u),
                    Count = _userService.Count()
                };
            }
            User.Types uType;
            try
            {
                uType = _mapper.Map<TypeT<User.Types>>(new TypeTString<User.Types> { Type = type }).Type;
            }
            catch (Exception)
            {
                throw new ErrBadReq($"Invalid Type `{type}`");
            }
            return new PaginatedResponse<IEnumerable<UserDtoGet>>
            {
                Value = from u in _userService.Get(uType, page, page_size) select _mapper.Map<UserDtoGet>(u),
                Count = _userService.Count(uType)
            };
        }

        [HttpGet("{uname}", Name = "GetByUName")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<UserDtoGet> Get(string uname)
        {
            User user = await _users.FindByNameAsync(uname) ?? throw new ErrNotFound("User Not Found");
            UserDtoGet res = _mapper.Map<UserDtoGet>(user);
            if (user.Type == hms.Models.User.Types.Doctor)
            {
                res.Doctor = await _doctors.GetByUName(user.UserName!);
            }
            else if (user.Type == hms.Models.User.Types.Patient)
            {
                res.Patient = await _patients.GetById(user.Id);
            }
            return res;
        }

        [HttpGet("whoami")]
        [Authorize]
        public async Task<UserDtoGet> WhoAmI()
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrForbidden();
            return await Get(user.UserName!);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<UserDtoGet>> Post([FromBody] UserDtoNew dto)
        {
            hms.Models.User.Types uType = _mapper.Map<TypeT<User.Types>>(
                new TypeTString<User.Types> { Type = dto.Type }).Type;
            User actor = await _users.GetUserAsync(User) ?? throw new ErrForbidden();
            if (uType == hms.Models.User.Types.Admin ||
                uType == hms.Models.User.Types.SuperAdmin)
            {
                if (actor.Type != hms.Models.User.Types.SuperAdmin)
                    throw new ErrForbidden();
            }
            User user = await _userService.Add(dto);
            UserDtoGet res = _mapper.Map<UserDtoGet>(user);
            return CreatedAtRoute("GetByUName", new { uname = user.UserName }, res);
        }

        [HttpDelete("{uname}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(string uname)
        {
            User subject = await _users.FindByNameAsync(uname) ?? throw new ErrNotFound("User Not Found");
            User actor = await _users.GetUserAsync(User) ?? throw new ErrForbidden();
            if (subject.Type == hms.Models.User.Types.SuperAdmin &&
                actor.Type == hms.Models.User.Types.Admin)
                throw new ErrForbidden("You cannot delete a SuperAdmin");
            if (actor.UserName == subject.UserName)
                throw new ErrBadReq("You cannot delete yourself");
            await _userService.SetActive(uname, false);
            return Ok();
        }
    }
}
