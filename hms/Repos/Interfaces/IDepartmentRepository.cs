using hms.Models;

namespace hms.Repos.Interfaces
{
    public interface IDepartmentRepository
    {
        public Task<int> Count();

        public Task<Department?> GetByUName(string uname);

        public Task<bool> ExistsByUName(string uname);

        public Task<IList<Department>> Get(int page = 1, int pageSize = 10);

        public Task Add(Department dept);

        public Task Update(Department dept);
        
        public Task Delete(Department depart);
    }
}
