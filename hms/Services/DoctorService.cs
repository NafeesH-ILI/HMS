using AutoMapper;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using hms.Utils;

namespace hms.Services
{
    public class DoctorService(
        IDoctorRepository doctorRepo,
        UserManager<User> userManager,
        IUserService userService,
        IUNameService namer,
        DbCtx ctx,
        IMapper mapper) : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepo = doctorRepo;
        private readonly UserManager<User> _users = userManager;
        private readonly IUserService _userService = userService;
        private readonly IUNameService _namer = namer;
        private readonly DbCtx _ctx = ctx;
        private readonly IMapper _mapper = mapper;
        public async Task<int> Count()
        {
            return await _doctorRepo.Count();
        }
        public async Task<int> Count(string fmt)
        {
            return await _doctorRepo.Count(fmt);
        }

        public async Task<Doctor> GetByUName(string uname)
        {
            return await _doctorRepo.GetByUName(uname) ?? throw new ErrNotFound();
        }
        public async Task<Doctor> GetById(string id)
        {
            return await _doctorRepo.GetById(id) ?? throw new ErrNotFound();
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _doctorRepo.ExistsByUName(uname);
        }
        public async Task<bool> ExistsById(string id)
        {
            return await _doctorRepo.ExistsById(id);
        }

        public async Task<IList<Doctor>> Get(int page = 1, int pageSize = 10)
        {
            return await _doctorRepo.Get(page, pageSize);
        }

        public async Task<Doctor> Add(DoctorDtoNew doctor)
        {
            Doctor d = _mapper.Map<Doctor>(doctor);
            User user = new()
            {
                UserName = _namer.Generate(doctor.Name),
                Type = User.Types.Doctor
            };
            if (!(await _users.CreateAsync(user,
                        doctor.Password ?? RandomPass.Password())).Succeeded)
            {
                throw new ErrBadReq("Username or Password does not meet criteria");
            }
            await _users.AddToRoleAsync(user, user.Type.ToString());
            await _ctx.SaveChangesAsync();
            d.Id = user.Id;
            try
            {
                await _doctorRepo.Add(d);
            } catch (Exception)
            {
                await _users.DeleteAsync(user);
                throw;
            }
            return d;
        }

        public async Task Update(string uname, DoctorDtoPut doctor)
        {
            Doctor d = await _doctorRepo.GetByUName(uname) ?? throw new ErrNotFound();
            _mapper.Map(doctor, d);
            await _doctorRepo.Update(d);
        }

        public async Task Update(string uname, DoctorDtoPatch doctor)
        {
            Doctor? d = await GetByUName(uname) ?? throw new ErrNotFound();
            _mapper.Map(doctor, d);
            await _doctorRepo.Update(d);
        }

        public async Task Delete(string uname)
        {
            await _doctorRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound());
        }

        public DoctorDtoGet ToDtoGet(Doctor doctor)
        {
            DoctorDtoGet dto = _mapper.Map<DoctorDtoGet>(doctor);
            dto.UName = _userService.UNameOf(doctor.Id)!;
            return dto;
        }
    }
}
