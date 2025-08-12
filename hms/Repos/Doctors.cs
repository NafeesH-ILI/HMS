using hms.Models;
using Microsoft.EntityFrameworkCore;

namespace hms.Repos
{
    public class Doctors (DbCtx ctx)
    {
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
            if (page < 1 || pageSize <= 0)
                throw new Exception("Invalid pagination");
            return await ctx.Doctors
                 .OrderBy(d => d.UName)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task Add(Doctor doctor)
        {
            if (doctor.UName == "")
                doctor.UName = UNamer.Generate("doctors", doctor.Name!);
            _ctx.Doctors.Add(doctor);
            await _ctx.SaveChangesAsync();
        }

        public async void Update(Doctor doctor)
        {
            _ctx.Doctors.Update(doctor);
            await _ctx.SaveChangesAsync();
        }
        
        public async void Delete(string uname)
        {
            _ctx.Doctors.Remove((await GetByUName(uname))!);
            await _ctx.SaveChangesAsync();
        }
    }
}
