using hms.Models;
using System.Security;

namespace hms.Repos.Interfaces
{
    public interface IPassResetRepository
    {
        public Task<PassResetOtp> Add(PassResetOtp otp);
        public Task<ICollection<PassResetOtp>> GetValid(string uname, string otp);
        public Task Invalidate(PassResetOtp otp);
    }
}
