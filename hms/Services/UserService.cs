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
        INameService namer,
        IMapper mapper,
        DbCtx ctx) : IUserService
    {
        private readonly UserManager<User> _users = users;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IPassResetService _passService = passService;
        private readonly INameService _namer = namer;
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

        public string? UNameOf(string id)
        {
            return _users.Users
                .Where(u => u.Id == id)
                .Select(u => u.UserName)
                .FirstOrDefault();
        }

        public async Task<User> Add(UserDtoNew dto)
        {
            _namer.ValidateName(dto.Name);
            User.Types uType = _mapper.Map<TypeT<User.Types>>(
                new TypeTString<User.Types> { Type = dto.Type }).Type;
            if (uType == User.Types.Patient ||
                uType == User.Types.Doctor)
            {
                throw new ErrBadReq("Cannot Add Patient or Doctor via this method");
            }
            User user = new()
            {
                UserName = _namer.Generate(dto.Name),
                Type = uType,
                IsActive = true
            };
            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded)
                throw new ErrBadReq("Username or Password does not meet critera");
            await _users.AddToRoleAsync(user, user.Type.ToString());
            await _ctx.SaveChangesAsync();
            return user;
        }

        public async Task PasswordChange(string uname, string password)
        {
            User user = await _users.FindByNameAsync(uname) ?? throw new ErrNotFound("User Not Found");
            string token = await _users.GeneratePasswordResetTokenAsync(user);
            var res = await _users.ResetPasswordAsync(user, token, password);
            if (!res.Succeeded)
                throw new ErrBadReq("Password does not meet critera");
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
            await _passService.Reset(Guid.Parse(dto.SessionId), dto.Otp, dto.Password);
        }
    }
}
