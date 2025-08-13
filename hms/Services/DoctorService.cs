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
        public Task Add(Doctor doctor);
        public Task Update(Doctor doctor);
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

        public async Task Add(Doctor doctor)
        {
            if (doctor.UName == "")
                doctor.UName = _namer.Generate("doctors", doctor.Name!);
            await _doctorRepo.Add(doctor);
        }

        public async Task Update(Doctor doctor)
        {
            await _doctorRepo.Update(doctor);
        }

        public async Task Delete(string uname)
        {
            await _doctorRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound());
        }
    }
}
