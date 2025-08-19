using hms.Models;
using hms.Models.DTOs;

namespace hms.Services.Interfaces
{
    public interface IUserService
    {
        public int Count();
        public int Count(User.Types type);
        public IList<User> Get(int page = 1, int pageSize = 10);
        public IList<User> Get(User.Types type, int page = 1, int pageSize = 10);
        public Task<User> Add(UserDtoNew dto);
        public Task PasswordChange(string uname, string password);
        public Task<PassResetOtp> PasswordReset(string uname);
        public Task PasswordReset(PasswordResetDto dto);
    }
}
