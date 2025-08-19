using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using AutoMapper;
using hms.Utils;

namespace hms.Services
{
    public class DepartmentService(
        DbCtx ctx,
        IMapper mapper,
        IDepartmentRepository deptRepo) : IDepartmentService
    {
        private readonly DbCtx _ctx = ctx;
        private readonly IMapper _mapper = mapper;
        private readonly IDepartmentRepository _deptRepo = deptRepo;

        public async Task<int> Count()
        {
            return await _deptRepo.Count();
        }

        public async Task<Department> GetByUName(string uname)
        {
            return await _deptRepo.GetByUName(uname) ?? throw new ErrNotFound();
        }

        public async Task<bool> ExistsByUName(string uname)
        {
            return await _deptRepo.ExistsByUName(uname);
        }

        public Task<IList<Department>> Get(int page = 1, int pageSize = 10)
        {
            return _deptRepo.Get(page, pageSize);
        }

        public async Task<Department> Add(DepartmentDtoNew dept)
        {
            Department d = _mapper.Map<Department>(dept);
            await _deptRepo.Add(d);
            return d;
        }

        public async Task Update(string uname, DepartmentDtoPut dept)
        {
            if (!await ExistsByUName(uname))
                throw new ErrNotFound();
            Department d = _mapper.Map<Department>(dept);
            d.UName = uname;
            await _deptRepo.Update(d);
        }

        public async Task Delete(string uname)
        {
            await _deptRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound());
        }
    }
}
