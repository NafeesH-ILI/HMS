using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using hms.Utils;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

namespace hms.Services
{
    public class PassResetService(
        UserManager<User> userManager,
        IPassResetRepository passRepo) : IPassResetService
    {
        private readonly IPassResetRepository _passRepo = passRepo;
        private readonly UserManager<User> _users = userManager;

        public async Task<PassResetOtp> New(string uname)
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
            return otp;
        }

        public async Task<PassResetOtp?> Validate(Guid id, string password)
        {
            PassResetOtp passReset = await _passRepo.Get(id) ?? throw new ErrUnauthorized();
            if (!passReset.IsValid || passReset.Expiry >= DateTime.Now.AddMinutes(Consts.OtpValidityMinutes))
                throw new ErrUnauthorized();
            await _passRepo.Invalidate(passReset);
            User user = await _users.FindByNameAsync(passReset.UName) ?? throw new ErrNotFound();
            string token = await _users.GeneratePasswordResetTokenAsync(user);
            await _users.ResetPasswordAsync(user, token, password);
            return passReset;
        }
    }
}
