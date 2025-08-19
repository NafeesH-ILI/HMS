using AutoMapper;
using hms.Models;
using hms.Models.DTOs;
using hms.Services.Interfaces;
using hms.Utils;
using Microsoft.AspNetCore.Identity;

namespace hms.Services
{
    public class UserService(
        UserManager<User> users,
        ILogger<UserService> logger,
        IPassResetService passService,
        IUNameService namer,
        IMapper mapper,
        DbCtx ctx) : IUserService
    {
        private readonly UserManager<User> _users = users;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IPassResetService _passService = passService;
        private readonly IUNameService _namer = namer;
        private readonly IMapper _mapper = mapper;
        private readonly DbCtx _ctx = ctx;

        public int Count()
        {
            return _users.Users.Count();
        }

        public int Count(User.Types type)
        {
            return _users.Users.Where(u => u.Type == type).Count();
        }

        public IList<User> Get(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return _users.Users
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IList<User> Get(User.Types type, int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return _users.Users
                .Where(u => u.Type == type)
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<User> Add(UserDtoNew dto)
        {
            User.Types uType = _mapper.Map<TypeType>(new TypeString { Type = dto.Type }).Type;
            if (uType == User.Types.Patient ||
                uType == User.Types.Doctor)
            {
                throw new ErrBadReq();
            }
            User user = new()
            {
                UserName = _namer.Generate(dto.Name),
                Type = uType
            };
            var res = await _users.CreateAsync(user);
            if (!res.Succeeded)
                throw new ErrBadReq();
            await _users.AddToRoleAsync(user, user.Type.ToString());
            await _ctx.SaveChangesAsync();
            return user;
        }

        public async Task PasswordChange(string uname, string password)
        {
            User user = await _users.FindByNameAsync(uname) ?? throw new ErrNotFound();
            string token = await _users.GeneratePasswordResetTokenAsync(user);
            var res = await _users.ResetPasswordAsync(user, token, password);
            if (!res.Succeeded)
                throw new Exception(res.Errors.ToString());
            await _ctx.SaveChangesAsync();
        }

        public async Task<PassResetOtp> PasswordReset(string uname)
        {
            PassResetOtp otp = await _passService.New(uname);
            _logger.LogError("Email sending not implemented, OTP for {uname}:{Id} is {Otp}", uname, otp.Id, otp.Otp);
            return otp;
        }

        public async Task PasswordReset(PasswordResetDto dto)
        {
            PassResetOtp otp = await _passService.Validate(dto.SessionId, dto.Otp) ?? throw new ErrUnauthorized();
            await PasswordChange(otp.UName, dto.Password);
        }
    }
}
