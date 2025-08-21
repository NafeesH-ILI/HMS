using hms.Models;
using hms.Repos.Interfaces;
using hms.Utils;
using Microsoft.EntityFrameworkCore;

namespace hms.Repos
{
    public class PatientRepository(DbCtx ctx) : IPatientRepository
    {
        private readonly DbCtx _ctx = ctx;

        public async Task<int> Count()
        {
            return await _ctx.Patients.CountAsync();
        }

        public async Task<Patient?> GetByUName(string uname)
        {
            return await _ctx.Patients
                .Include(p => p.User)
                .Where(p => p.User.UserName == uname)
                .FirstOrDefaultAsync();
        }
        public async Task<Patient?> GetById(string id)
        {
            return await _ctx.Patients.FindAsync(id);
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _ctx.Patients
                .Include(p => p.User)
                .Where(p => p.User.UserName == uname)
                .CountAsync() > 0;
        }

        public async Task<bool> ExistsById(string id)
        {
            return await _ctx.Patients
                .Where(p => p.Id == id)
                .AnyAsync();
        }

        public async Task<IList<Patient>> Get(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Patients
                .OrderBy(p => p.Phone)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IList<Patient>> GetByPhone(string phone, int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Patients
                .Where(p => p.Phone == phone)
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Add(Patient patient)
        {
            _ctx.Patients.Add(patient);
            await _ctx.SaveChangesAsync();
        }

        public async Task Update(Patient patient)
        {
            _ctx.Patients.Update(patient);
            await _ctx.SaveChangesAsync();
        }

        public async Task Delete(Patient patient)
        {
            _ctx.Patients.Remove(patient);
            await _ctx.SaveChangesAsync();
        }
    }
}
