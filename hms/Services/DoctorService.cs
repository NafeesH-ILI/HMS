using hms.Models;
using hms.Repos;

namespace hms.Services
{
    public interface IDoctorService
    {
        public Task<int> Count(string? fmt = null);
        public Task<Doctor> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Doctor>> Get(int page = 1, int pageSize = 10);
        public Task Add(Doctor doctor);
        public Task Update(Doctor doctor);
        public Task Delete(string uname);
    }
    public class DoctorService(
        IDoctorRepository doctorRepo) : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepo = doctorRepo;
        public async Task<int> Count(string? fmt = null)
        {
            return await _doctorRepo.Count(fmt);
        }

        public async Task<Doctor> GetByUName(string uname)
        {
            return await _doctorRepo.GetByUName(uname);
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
            await _doctorRepo.Add(doctor);
        }

        public async Task Update(Doctor doctor)
        {
            await _doctorRepo.Update(doctor);
        }

        public async Task Delete(string uname)
        {
            await _doctorRepo.Delete(uname);
        }
    }
}
