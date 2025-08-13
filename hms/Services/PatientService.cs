using hms.Repos;
using hms.Models;

namespace hms.Services
{
    public interface IPatientService
    {
        public Task<int> Count();
        public Task<int> CountByPhone(string phone);
        public Task<IList<Patient>> GetByPhone(string phone, int page = 1, int pageSize = 10);
        public Task<Patient> GetByPhoneName(string phone, string name);
        public Task<bool> ExistsByPhoneName(string phone, string name);
        public Task<IList<Patient>> Get(int page = 1, int pageSize = 10);
        public Task<Patient> Add(PatientDto patient);
        public Task Update(string phone, string name, PatientDto patientDto);
        public Task Delete(string phone, string name);
    }
    public class PatientService(
        IPatientRepository patientRepo) : IPatientService
    {
        private readonly IPatientRepository _patientRepo = patientRepo;
        public async Task<int> Count()
        {
            return await _patientRepo.Count();
        }

        public async Task<int> CountByPhone(string phone)
        {
            return await _patientRepo.CountByPhone(phone);
        }

        public async Task<IList<Patient>> GetByPhone(string phone, int page = 1, int pageSize = 10)
        {
            return await _patientRepo.GetByPhone(phone, page, pageSize);
        }

        public async Task<Patient> GetByPhoneName(string phone, string name)
        {
            return await _patientRepo.GetByPhoneName(phone, name) ?? throw new ErrNotFound();
        }

        public async Task<bool> ExistsByPhoneName(string phone, string name)
        {
            return await _patientRepo.ExistsByPhoneName(phone, name);
        }
        
        public async Task<IList<Patient>> Get(int page = 1, int pageSize = 10)
        {
            return await _patientRepo.Get(page, pageSize);
        }

        public async Task<Patient> Add(PatientDto patientDto)
        {
            Patient p = new()
            {
                Phone = patientDto.Phone,
                Name = patientDto.Name,
                DateBirth = patientDto.DateBirth
            };
            await _patientRepo.Add(p);
            return p;
        }

        public async Task Update(string phone, string name, PatientDto patientDto)
        {
            Patient p = await GetByPhoneName(phone, name);
            p.Phone = patientDto.Phone;
            p.Name = patientDto.Name;
            p.DateBirth = patientDto.DateBirth;
            await _patientRepo.Update(p);
        }

        public async Task Delete(string phone, string name)
        {
            await _patientRepo.Delete(await _patientRepo.GetByPhoneName(phone, name) ?? throw new ErrNotFound());
        }
    }
}
