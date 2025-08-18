using AutoMapper;
using hms.Common;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using System.Security.Cryptography.Xml;

namespace hms.Services
{
    public class UserService(
        IMapper mapper,
        IDoctorService doctorService,
        IPatientService patientService,
        IUNameService namer,
        IUserRepository userRepo) : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IDoctorService _doctorService = doctorService;
        private readonly IPatientService _patientService = patientService;
        private readonly IUNameService _namer = namer;
        private readonly IUserRepository _userRepo = userRepo;

        private bool CanAffect(User.Types actor, User.Types subject)
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
        public async Task<User> Add(string actorUName, UserDtoNew userNew)
        {
            User actor = await _userRepo.GetByUName(actorUName) ?? throw new ErrUnauthorized();
            User user = _mapper.Map<User>(userNew);
            user.UName = userNew.Name;
            user.PassHash = userNew.Password; // TODO: hash the password

            if (!CanAffect(actor.Type, user.Type))
                throw new ErrUnauthorized();

            if (user.Type == User.Types.Doctor)
            {
                if (!await _doctorService.ExistsByUName(user.UName))
                    throw new ErrBadReq();
                if (await _userRepo.ExistsByUName(user.UName))
                    throw new ErrAlreadyExists();
            }
            else if (user.Type == User.Types.Patient)
            {
                if (!await _patientService.ExistsByUName(user.UName))
                    throw new ErrBadReq();
                if (await _userRepo.ExistsByUName(user.UName))
                    throw new ErrAlreadyExists();
            }
            else
            {
                user.UName = _namer.Generate(user.UName);
            }

            await _userRepo.Add(user);
            return user;
        }
        public async Task UpdatePassword(string actorUName, string uname, string password)
        {
            User actor = await _userRepo.GetByUName(actorUName) ?? throw new ErrUnauthorized();
            User user = await GetByUName(uname) ?? throw new ErrNotFound();
            if (!CanAffect(actor.Type, user.Type))
                throw new ErrUnauthorized();
            user.PassHash = password; // TODO: hash the password
            await _userRepo.Update(user);
        }
        public async Task Delete(string actorUName, string uname)
        {
            User actor = await _userRepo.GetByUName(actorUName) ?? throw new ErrUnauthorized();
            if (actor.Type != User.Types.SuperAdmin)
                throw new ErrUnauthorized();
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
