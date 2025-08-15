using hms.Common;
using hms.Models;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hms.Repos
{
    public class DoctorRepository (IUNameService namer, DbCtx ctx) : IDoctorRepository
    {
        private readonly IUNameService _namer = namer;
        private readonly DbCtx _ctx = ctx;

        public async Task<int> Count()
        {
            return await _ctx.Doctors.CountAsync();
        }

        public async Task<int> Count(string fmt)
        {
            return await _ctx.Doctors
                .Where(d => d.Name != null && d.Name.Contains(fmt))
                .CountAsync();
        }

        public async Task<Doctor?> GetByUName(string uname)
        {
            return await _ctx.Doctors
                .Where(d => d.UName == uname)
                .Include(d => d.Dept)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _ctx.Doctors
                .Where(d => d.UName == uname)
                .CountAsync() > 0;
        }

        public async Task<IList<Doctor>> Get(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Doctors
                 .OrderBy(d => d.UName)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task Add(Doctor doctor)
        {
            _ctx.Doctors.Add(doctor);
            await _ctx.SaveChangesAsync();
        }

        public async Task Update(Doctor doctor)
        {
            _ctx.Doctors.Update(doctor);
            await _ctx.SaveChangesAsync();
        }
        
        public async Task Delete(Doctor doctor)
        {
            _ctx.Doctors.Remove(doctor);
            await _ctx.SaveChangesAsync();
        }
    }
}
