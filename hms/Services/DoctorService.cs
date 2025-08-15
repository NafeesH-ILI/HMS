using hms.Common;
using AutoMapper;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;

namespace hms.Services
{
    public class DoctorService(
        IDoctorRepository doctorRepo,
        IMapper mapper,
        IUNameService namer) : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepo = doctorRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IUNameService _namer = namer;
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

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _doctorRepo.ExistsByUName(uname);
        }

        public async Task<IList<Doctor>> Get(int page = 1, int pageSize = 10)
        {
            return await _doctorRepo.Get(page, pageSize);
        }

        public async Task<Doctor> Add(DoctorDtoNew doctor)
        {
            Doctor d = _mapper.Map<Doctor>(doctor);
            d.UName = _namer.Generate("persons", doctor.Name);
            await _doctorRepo.Add(d);
            return d;
        }

        public async Task Update(string uname, DoctorDtoNew doctor)
        {
            if (!await ExistsByUName(uname))
                throw new ErrNotFound();
            Doctor d = _mapper.Map<Doctor>(doctor);
            d.UName = uname;
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
    }
}
