using hms.Common;
using hms.Models;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;

namespace hms.Services
{
    public class UserService(
        IUserRepository userRepo) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;
        public async Task<int> Count()
        {
            return await _userRepo.Count();
        }
        public async Task<int> CountByType(User.Types type)
        {
            return await _userRepo.CountByType(type);
        }
        public async Task<IList<User>> Get(int page = 1, int pageSize = 10)
        {
            return await _userRepo.Get(page, pageSize);
        }
        public async Task<IList<User>> GetByType(User.Types type, int page = 1, int pageSize = 10)
        {
            return await _userRepo.GetByType(type, page, pageSize);
        }
        public async Task<bool> ExistsByUName(string uname)
        {
            return await _userRepo.ExistsByUName(uname);
        }
        public async Task<User> GetByUName(string uname)
        {
            return await _userRepo.GetByUName(uname) ?? throw new ErrNotFound();
        }
        public async Task Add(User user)
        {
            // TODO: hash the password
            await _userRepo.Add(user);
        }
        public async Task Update(User user)
        {
            // TODO: check if password not hashed, hash it
            await _userRepo.Update(user);
        }
        public async Task Delete(User user)
        {
            await _userRepo.Delete(user);
        }
        public async Task<bool> Authenticate(string uname, string password)
        {
            User? user = await _userRepo.GetByUName(uname);
            if (user == null)
                return false;
            // TODO: do password hashing
            return password == user.PassHash;
        }
    }
}
