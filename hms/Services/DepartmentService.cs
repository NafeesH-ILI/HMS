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
        public Task<Department> Add(DepartmentDtoNew dept);
        public Task Update(string uname, DepartmentDtoPut dept);
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
            return await _deptRepo.GetByUName(uname) ?? throw new ErrNotFound();
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _deptRepo.ExistsByUName(uname);
        }

        public Task<IList<Department>> Get(int page = 1, int pageSize = 10)
        {
            return _deptRepo.Get(page, pageSize);
        }

        public async Task<Department> Add(DepartmentDtoNew dept)
        {
            Department d = new()
            {
                UName = dept.UName,
                Name = dept.Name,
            };
            await _deptRepo.Add(d);
            return d;
        }

        public async Task Update(string uname, DepartmentDtoPut dept)
        {
            if (!await ExistsByUName(uname))
                throw new ErrNotFound();
            Department d = await GetByUName(uname);
            d.Name = dept.Name;
            await _deptRepo.Update(d);
        }

        public async Task Delete(string uname)
        {
            await _deptRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound());
        }
    }
}
