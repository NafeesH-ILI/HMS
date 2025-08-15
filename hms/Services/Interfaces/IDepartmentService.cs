using hms.Models;
using hms.Models.DTOs;

namespace hms.Services.Interfaces
{
    public interface IDepartmentService
    {
        public Task<int> Count();
        public Task<Department> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Department>> Get(int page = 1, int pageSize = 10);
        public Task<Department> Add(DepartmentDtoNew dept);
        public Task Update(string uname, DepartmentDtoPut dept);
        public Task Delete(string uname);
    }
}
