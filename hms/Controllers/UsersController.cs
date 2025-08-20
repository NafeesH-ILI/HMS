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
                uType = _mapper.Map<TypeType>(new TypeString { Type = type }).Type;
            }
            catch (Exception)
            {
                throw new ErrBadReq();
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
            User user = await _users.FindByNameAsync(uname) ?? throw new ErrNotFound();
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
            User user = await _users.GetUserAsync(User) ?? throw new ErrUnauthorized();
            return await Get(user.UserName!);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<UserDtoGet>> Post([FromBody] UserDtoNew dto)
        {
            hms.Models.User.Types uType = _mapper.Map<TypeType>(new TypeString { Type = dto.Type }).Type;
            User actor = await _users.GetUserAsync(User) ?? throw new ErrUnauthorized();
            if (uType == hms.Models.User.Types.Admin ||
                uType == hms.Models.User.Types.SuperAdmin)
            {
                if (actor.Type != hms.Models.User.Types.SuperAdmin)
                    throw new ErrUnauthorized();
            }
            User user = await _userService.Add(dto);
            UserDtoGet res = _mapper.Map<UserDtoGet>(user);
            return CreatedAtRoute("GetByUName", new { uname = user.UserName }, res);
        }
    }
}
