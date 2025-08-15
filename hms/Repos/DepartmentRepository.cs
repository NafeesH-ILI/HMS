using hms.Common;
using hms.Models;
using hms.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hms.Repos
{
    public class DepartmentRepository(DbCtx ctx) : IDepartmentRepository
    {
        private readonly DbCtx _ctx = ctx;

        public async Task<int> Count()
        {
            return await _ctx.Departments.CountAsync();
        }

        public async Task<Department?> GetByUName(string uname)
        {
            return await _ctx.Departments
                .Where(d => d.UName == uname)
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _ctx.Departments
                .Where(d => d.UName == uname)
                .CountAsync() > 0;
        }

        public async Task<IList<Department>> Get(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Departments
                 .OrderBy(d => d.UName)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task Add(Department dept)
        {
            _ctx.Departments.Add(dept);
            await _ctx.SaveChangesAsync();
        }

        public async Task Update(Department dept)
        {
            _ctx.Departments.Update(dept);
            await _ctx.SaveChangesAsync();
        }
        
        public async Task Delete(Department dept)
        {
            _ctx.Departments.Remove(dept);
            await _ctx.SaveChangesAsync();
        }
    }
}
