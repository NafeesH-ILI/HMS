using hms.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace hms.Repos
{
    public interface IPatientRepository
    {
        public Task<int> Count();
        public Task<int> CountByPhone(string phone);
        public Task<IList<Patient>> GetByPhone(string phone, int page = 1, int pageSize = 10);
        public Task<Patient> GetByPhoneName(string phone, string name);
        public Task<bool> ExistsByPhoneName(string phone, string name);
        public Task<IList<Patient>> Get(int page = 1, int pageSize = 10);
        public Task Add(Patient patient);
        public Task Update(Patient patient);
        public Task Delete(string phone, string name);
    }
    
    public class PatientRepository(DbCtx ctx) : IPatientRepository
    {
        private readonly DbCtx _ctx = ctx;

        public async Task<int> Count()
        {
            return await _ctx.Patients.CountAsync();
        }
        public async Task<int> CountByPhone(string phone)
        {
            return await _ctx.Patients
                .Where(p => p.Phone == phone)
                .CountAsync();
        }

        public async Task<IList<Patient>> GetByPhone(string phone, int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Patients
                .Where(p => p.Phone == phone)
                .OrderBy(p => new {p.Phone, p.Name})
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Patient> GetByPhoneName(string phone, string name)
        {
            return await _ctx.Patients
                .Where(p => p.Phone == phone && p.Name == name)
                .FirstOrDefaultAsync() ?? throw new ErrNotFound();
        }

        public async Task<bool> ExistsByPhoneName(string phone, string name)
        {
            return await _ctx.Patients
                .Where(p => p.Phone == phone && p.Name == name)
                .CountAsync() > 0;
        }

        public async Task<IList<Patient>> Get(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Patients
                .OrderBy(p => new {p.Phone, p.Name})
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

        public async Task Delete(string phone, string name)
        {
            _ctx.Patients.Remove(await GetByPhoneName(phone, name));
            await _ctx.SaveChangesAsync();
        }
    }
}
