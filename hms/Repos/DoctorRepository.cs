using hms.Models;
using hms.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace hms.Repos
{
    public interface IDoctorRepository
    {
        public Task<int> Count(string? fmt = null);

        public Task<Doctor> GetByUName(string uname);

        public Task<bool> ExistsByUName(string uname);

        public Task<IList<Doctor>> Get(int page = 1, int pageSize = 10);

        public Task Add(Doctor doctor);

        public Task Update(Doctor doctor);
        
        public Task Delete(string uname);
    }
    public class DoctorRepository (IUNameService namer, DbCtx ctx) : IDoctorRepository
    {
        private readonly IUNameService _namer = namer;
        private readonly DbCtx _ctx = ctx;

        public async Task<int> Count(string? fmt = null)
        {
            if (fmt == null)
            {
                return await _ctx.Doctors.CountAsync();
            }
            return await _ctx.Doctors
            .Where(d => d.Name != null && d.Name.Contains(fmt))
            .CountAsync();
        }

        public async Task<Doctor> GetByUName(string uname)
        {
            return await _ctx.Doctors
                .Where(d => d.UName == uname)
                .Include(d => d.Dept)
                .FirstOrDefaultAsync() ?? throw new ErrNotFound();
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
            return await ctx.Doctors
                 .OrderBy(d => d.UName)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task Add(Doctor doctor)
        {
            if (doctor.UName == "")
                doctor.UName = _namer.Generate("doctors", doctor.Name!);
            _ctx.Doctors.Add(doctor);
            await _ctx.SaveChangesAsync();
        }

        public async Task Update(Doctor doctor)
        {
            _ctx.Doctors.Update(doctor);
            await _ctx.SaveChangesAsync();
        }
        
        public async Task Delete(string uname)
        {
            _ctx.Doctors.Remove((await GetByUName(uname)) ?? throw new ErrNotFound());
            await _ctx.SaveChangesAsync();
        }
    }
}
