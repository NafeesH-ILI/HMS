using hms.Models;
using System.Security;

namespace hms.Repos.Interfaces
{
    public interface IPassResetRepository
    {
        public Task<PassResetOtp> Add(PassResetOtp otp);
        public Task<int> CountValid(string uname);
        public Task<PassResetOtp?> Get(Guid id);
        public Task Invalidate(PassResetOtp otp);
        public Task Cleanup();
    }
}
