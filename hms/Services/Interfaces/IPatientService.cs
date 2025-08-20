using hms.Models;
using hms.Models.DTOs;

namespace hms.Services.Interfaces
{
    public interface IPatientService
    {
        public Task<int> Count();
        public Task<Patient> GetByUName(string uname);
        public Task<Patient> GetById(string id);
        public Task<bool> ExistsByUName(string uname);
        public Task<bool> ExistsById(string id);
        public Task<IList<Patient>> Get(int page = 1, int pageSize = 10);
        public Task<Patient> Add(PatientDtoNew patient);
        public Task Update(string id, PatientDtoPut patientDto);
        public Task Update(string id, PatientDtoPatch patientDto);
        public Task Delete(string id);
        public PatientDtoGet ToDtoGet(Patient patient);
    }
}
