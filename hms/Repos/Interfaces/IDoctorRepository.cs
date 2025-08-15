using hms.Models;

namespace hms.Repos.Interfaces
{
    public interface IDoctorRepository
    {
        public Task<int> Count();
        public Task<int> Count(string fmt);

        public Task<Doctor?> GetByUName(string uname);

        public Task<bool> ExistsByUName(string uname);

        public Task<IList<Doctor>> Get(int page = 1, int pageSize = 10);

        public Task Add(Doctor doctor);

        public Task Update(Doctor doctor);
        
        public Task Delete(Doctor doctor);
    }
}
