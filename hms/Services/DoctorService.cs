using hms.Models;
using hms.Repos;

namespace hms.Services
{
    public interface IDoctorService
    {
        public Task<int> Count();
        public Task<int> Count(string fmt);
        public Task<Doctor> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Doctor>> Get(int page = 1, int pageSize = 10);
        public Task<Doctor> Add(DoctorDtoNew doctor);
        public Task Update(string uname, DoctorDtoNew doctor);
        public Task Update(string uname, DoctorDtoPatch doctor);
        public Task Delete(string uname);
    }
    public class DoctorService(
        IDoctorRepository doctorRepo,
        IUNameService namer) : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepo = doctorRepo;
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
            Doctor d = new() {
                UName = _namer.Generate("doctors", doctor.Name!),
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey,
            };
            await _doctorRepo.Add(d);
            return d;
        }

        public async Task Update(string uname, DoctorDtoNew doctor)
        {
            if (!await ExistsByUName(uname))
                throw new ErrNotFound();
            Doctor d = new()
            {
                UName = uname,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey
            };
            await _doctorRepo.Update(d);
        }

        public async Task Update(string uname, DoctorDtoPatch doctor)
        {
            Doctor? d = await GetByUName(uname);
            if (d == null)
                throw new ErrNotFound();
            if (doctor.Name != null)
                d.Name = doctor.Name;
            if (doctor.Specialization != null)
                d.Specialization = doctor.Specialization;
            if (doctor.MaxQualification != null)
                d.MaxQualification = doctor.MaxQualification;
            if (doctor.DeptKey != null)
                d.DeptKey = doctor.DeptKey;
            await _doctorRepo.Update(d);
        }

        public async Task Delete(string uname)
        {
            await _doctorRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound());
        }
    }
}
