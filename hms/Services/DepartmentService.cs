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
        INameService namer,
        IDepartmentRepository deptRepo) : IDepartmentService
    {
        private readonly DbCtx _ctx = ctx;
        private readonly IMapper _mapper = mapper;
        private readonly INameService _namer = namer;
        private readonly IDepartmentRepository _deptRepo = deptRepo;

        public async Task<int> Count()
        {
            return await _deptRepo.Count();
        }

        public async Task<Department> GetByUName(string uname)
        {
            return await _deptRepo.GetByUName(uname) ?? throw new ErrNotFound("Department Not Found");
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
            if (dept.UName.Length < Consts.UNameMinLen)
                throw new ErrBadReq($"Departmment UName cannot be less than {Consts.UNameMinLen} characters");
            _namer.ValidateName(dept.Name);
            Department d = _mapper.Map<Department>(dept);
            await _deptRepo.Add(d);
            return d;
        }

        public async Task Update(string uname, DepartmentDtoPut dept)
        {
            _namer.ValidateName(dept.Name);
            Department? d = await GetByUName(uname) ?? throw new ErrNotFound("Department Not Found");
            _mapper.Map(dept, d);
            await _deptRepo.Update(d);
        }

        public async Task Delete(string uname)
        {
            await _deptRepo.Delete(await GetByUName(uname) ?? throw new ErrNotFound("Department Not Found"));
        }
    }
}
