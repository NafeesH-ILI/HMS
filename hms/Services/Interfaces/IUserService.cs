using hms.Models;
using hms.Models.DTOs;

namespace hms.Services.Interfaces
{
    public interface IUserService
    {
        public Task<int> Count();
        public Task<int> CountByType(User.Types type);
        public Task<IList<User>> Get(int page = 1, int pageSize = 10);
        public Task<IList<User>> GetByType(User.Types type, int page = 1, int pageSize = 10);
        public Task<bool> ExistsByUName(string uname);
        public Task<User> GetByUName(string uname);
        public Task<User> Add(string actorUName, UserDtoNew user);
        public Task UpdatePassword(string actorUName, string uname, string password);
        public Task Delete(string actorUName, string uname);
        public Task<bool> Authenticate(string uname, string password);
    }
}
