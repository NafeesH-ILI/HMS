using hms.Models;

namespace hms.Services.Interfaces
{
    public interface IDoctorService
    {
        public Task<int> Count();
        public Task<int> Count(string fmt);
        public Task<Doctor> GetByUName(string uname);
        public Task<bool> ExistsByUName(string uname);
        public Task<IList<Doctor>> Get(int page = 1, int pageSize = 10);
        public Task<Doctor> Add(DoctorDtoNew doctor);
        public Task Update(string uname, DoctorDtoNew doctor);
        public Task Update(string uname, DoctorDtoPatch doctor);
        public Task Delete(string uname);
    }
}
