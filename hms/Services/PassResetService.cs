using hms.Models;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using hms.Utils;
using System.Reflection.Metadata.Ecma335;

namespace hms.Services
{
    public class PassResetService(
        IPassResetRepository passRepo) : IPassResetService
    {
        private readonly IPassResetRepository _passRepo = passRepo;

        public async Task<string> New(string uname)
        {
            PassResetOtp otp = new()
            {
                Id = Guid.NewGuid(),
                UName = uname,
                IsValid = true,
                Expiry = DateTime.Now.Add(TimeSpan.FromMinutes(2)),
                Otp = RandomPass.Otp()
            };
            await _passRepo.Add(otp);
            return otp.Otp;
        }

        public async Task<bool> Validate(string uname, string otp)
        {
            ICollection<PassResetOtp> otpList = await _passRepo.GetValid(uname, otp);
            if (otpList.Count() == 0)
                return false;
            foreach (PassResetOtp obj in otpList)
                await _passRepo.Invalidate(obj);
            return true;
        }
    }
}
