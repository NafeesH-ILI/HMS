using hms.Repos;
using hms.Models;

namespace hms.Services
{
    public interface IDepartmentService
    {
        public Task<int> Count();
        public Task<Department> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Department>> Get(int page = 1, int pageSize = 10);
        public Task Add(Department dept);
        public Task Update(Department dept);
        public Task Delete(string uname);
    }
    public class DepartmentService(
        DbCtx ctx,
        IDepartmentRepository deptRepo) : IDepartmentService
    {
        private readonly DbCtx _ctx = ctx;
        private readonly IDepartmentRepository _deptRepo = deptRepo;

        public async Task<int> Count()
        {
            return await _deptRepo.Count();
        }

        public async Task<Department> GetByUName(string uname)
        {
            return await _deptRepo.GetByUName(uname);
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _deptRepo.ExistsByUName(uname);
        }

        public Task<IList<Department>> Get(int page = 1, int pageSize = 10)
        {
            return _deptRepo.Get(page, pageSize);
        }

        public async Task Add(Department dept)
        {
            await _deptRepo.Add(dept);
        }

        public async Task Update(Department dept)
        {
            await _deptRepo.Update(dept);
        }

        public async Task Delete(string uname)
        {
            await _deptRepo.Delete(uname);
        }
    }
}
