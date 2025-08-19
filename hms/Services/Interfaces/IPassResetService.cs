using hms.Models;

namespace hms.Services.Interfaces
{
    public interface IPassResetService
    {
        public Task<string> New(string uname);
        public Task<bool> Validate(string uname, string otp);
    }
}
