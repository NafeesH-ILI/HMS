using hms.Models;
using hms.Models.DTOs;

namespace hms.Services.Interfaces
{
    public interface IPatientService
    {
        public Task<int> Count();
        public Task<Patient> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Patient>> Get(int page = 1, int pageSize = 10);
        public Task<Patient> Add(PatientDtoNew patient);
        public Task Update(string uname, PatientDtoNew patientDto);
        public Task Update(string uname, PatientDtoPatch patientDto);
        public Task Delete(string uname);
    }
}
