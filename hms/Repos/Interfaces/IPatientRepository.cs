using hms.Models;

namespace hms.Repos.Interfaces
{
    public interface IPatientRepository
    {
        public Task<int> Count();
        public Task<Patient?> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Patient>> Get(int page = 1, int pageSize = 10);
        public Task Add(Patient patient);
        public Task Update(Patient patient);
        public Task Delete(Patient patient);
    }
}
