using hms.Models;

namespace hms.Services.Interfaces
{
    public interface IPassResetService
    {
        public Task<PassResetOtp> New(string uname);
        public Task<PassResetOtp?> Validate(Guid id, string otp);
    }
}
