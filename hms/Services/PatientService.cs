using hms.Utils;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace hms.Services
{
    public class PatientService(
        IPatientRepository patientRepo,
        INameService namer,
        IUserService userService,
        UserManager<User> userManager,
        DbCtx ctx,
        IMapper mapper) : IPatientService
    {
        private readonly IPatientRepository _patientRepo = patientRepo;
        private readonly INameService _namer = namer;
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _users = userManager;
        private readonly DbCtx _ctx = ctx;
        private readonly IMapper _mapper = mapper;
        public async Task<int> Count()
        {
            return await _patientRepo.Count();
        }
        public async Task<Patient> GetByUName(string uname)
        {
            return await _patientRepo.GetByUName(uname) ?? throw new ErrNotFound("Patient Not Found");
        }

        public async Task<Patient> GetById(string id)
        {
            return await _patientRepo.GetById(id) ?? throw new ErrNotFound("Patient Not Found");
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _patientRepo.ExistsByUName(uname);
        }
        public async Task<bool> ExistsById(string id)
        {
            return await _patientRepo.ExistsById(id);
        }

        public async Task<IList<Patient>> Get(int page = 1, int pageSize = 10)
        {
            return await _patientRepo.Get(page, pageSize);
        }

        public async Task<Patient> Add(PatientDtoNew patientDto)
        {
            _namer.ValidateName(patientDto.Name);
            Patient p = _mapper.Map<Patient>(patientDto);
            User user = new()
            {
                UserName = _namer.Generate(patientDto.Name),
                Type = User.Types.Patient,
                IsActive = true
            };
            if (!(await _users.CreateAsync(user,
                        patientDto.Password ?? RandomPass.Password())).Succeeded)
            {
                throw new ErrBadReq("Username or Password does not meet critera");
            }
            await _users.AddToRoleAsync(user, user.Type.ToString());
            await _ctx.SaveChangesAsync();
            p.Id = user.Id;
            try
            {
                await _patientRepo.Add(p);
            }
            catch (Exception)
            {
                await _users.DeleteAsync(user);
                throw;
            }
            return p;
        }

        public async Task Update(string id, PatientDtoPut patientDto)
        {
            _namer.ValidateName(patientDto.Name);
            Patient p = await GetById(id);
            _mapper.Map(patientDto, p);
            await _patientRepo.Update(p);
        }

        public async Task Update(string id, PatientDtoPatch patientDto)
        {
            if (patientDto.Name != null)
                _namer.ValidateName(patientDto.Name);
            Patient p = await GetById(id);
            _mapper.Map(patientDto, p);
            await _patientRepo.Update(p);
        }

        public async Task Delete(string id)
        {
            await _patientRepo.Delete(await _patientRepo.GetById(id) ??
                throw new ErrNotFound("Patient Not Found"));
        }

        public PatientDtoGet ToDtoGet(Patient patient)
        {
            PatientDtoGet dto = _mapper.Map<PatientDtoGet>(patient);
            dto.UName = _userService.UNameOf(patient.Id)!;
            return dto;
        }
    }
}
