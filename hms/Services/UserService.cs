using AutoMapper;
using hms.Common;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using hms.Utils;
using System.Security.Cryptography.Xml;

namespace hms.Services
{
    public class UserService(
        IUNameService namer,
        IUserRepository userRepo) : IUserService
    {
        private readonly IUNameService _namer = namer;
        private readonly IUserRepository _userRepo = userRepo;

        private static bool CanAffect(User.Types actor, User.Types subject)
        {
            if (actor == User.Types.Receptionist)
            {
                if (subject != User.Types.Patient)
                    return false;
            }
            else if (actor == User.Types.Admin)
            {
                if (subject == User.Types.SuperAdmin)
                    return false;
            }
            else if (subject != User.Types.SuperAdmin)
            {
                return false;
            }
            return true;
        }

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

        public async Task<User> Add(string name, User.Types type, string? password = null)
        {
            password ??= RandomPass.RandomPassword();
            User user = new()
            {
                UName = _namer.Generate(name),
                Type = type,
                PassHash = password, // TODO: hash the password
            };

            await _userRepo.Add(user);
            return user;
        }

        public async Task UpdatePassword(string uname, string password)
        {
            User user = await GetByUName(uname) ?? throw new ErrNotFound();
            user.PassHash = password; // TODO: hash the password
            await _userRepo.Update(user);
        }

        public async Task Delete(string uname)
        {
            await _userRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound());
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
