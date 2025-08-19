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

        public async Task<int> CountValid(string uname)
        {
            return await _ctx.Otps
                .Where(o => o.UName == uname)
                .Where(o => o.IsValid)
                .Where(o => o.Expiry < DateTime.Now.AddMinutes(Consts.OtpValidityMinutes))
                .CountAsync();
        }

        public async Task<PassResetOtp?> Get(Guid id)
        {
            return await _ctx.Otps.FindAsync(id);
        }

        public async Task Invalidate(PassResetOtp otp)
        {
            otp.IsValid = false;
            _ctx.Otps.Update(otp);
            await _ctx.SaveChangesAsync();
        }
    }
}
