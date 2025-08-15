using hms.Repos.Interfaces;
using hms.Models;
using Microsoft.EntityFrameworkCore;
using hms.Common;

namespace hms.Repos
{
    public class UserRepository(
        DbCtx ctx) : IUserRepository
    {
        private readonly DbCtx _ctx = ctx;

        public async Task<int> Count()
        {
            return await _ctx.Users.CountAsync();
        }

        public async Task<int> CountByType(User.Types type)
        {
            return await _ctx.Users
                .Where(u => u.Type == type)
                .CountAsync();
        }

        public async Task<IList<User>> Get(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Users
                .OrderBy(u => u.UName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IList<User>> GetByType(User.Types type, int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 50)
                throw new ErrBadPagination();
            return await _ctx.Users
                .Where(u => u.Type == type)
                .OrderBy(u => u.UName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _ctx.Users
                .Where(u => u.UName == uname)
                .AnyAsync();
        }

        public async Task<User?> GetByUName(string uname)
        {
            return await _ctx.Users.FindAsync(uname);
        }
        
        public async Task Add(User user)
        {
            await _ctx.Users.AddAsync(user);
        }

        public async Task Update(User user)
        {
            _ctx.Users.Update(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task Delete(User user)
        {
            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
        }
    }
}
