using hms.Models;
using hms.Repos.Interfaces;
using hms.Utils;
using Microsoft.EntityFrameworkCore;

namespace hms.Repos
{
    public class PassResetRepository(
        DbCtx ctx) : IPassResetRepository
    {
        private readonly DbCtx _ctx = ctx;

        public async Task<PassResetOtp> Add(PassResetOtp otp)
        {
            _ctx.Otps.Add(otp);
            await _ctx.SaveChangesAsync();
            return otp;
        }

        public async Task<ICollection<PassResetOtp>> GetValid(string uname, string otp)
        {
            return await _ctx.Otps.Where(o => o.Expiry < DateTime.Now && o.UName == uname && o.Otp == otp).ToListAsync();
        }

        public async Task Invalidate(PassResetOtp otp)
        {
            otp.IsValid = false;
            _ctx.Otps.Update(otp);
            await _ctx.SaveChangesAsync();
        }
    }
}
