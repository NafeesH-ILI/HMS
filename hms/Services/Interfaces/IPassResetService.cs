using hms.Models;

namespace hms.Services.Interfaces
{
    public interface IPassResetService
    {
        public Task<PassResetOtp> New(string uname);
        public Task Reset(Guid id, string otp, string password);
    }
}
