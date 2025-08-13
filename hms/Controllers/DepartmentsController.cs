using hms.Models;
using hms.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/departments")]
    [ErrorHandler]
    public class DepartmentsController(
        ILogger<DepartmentsController> logger,
        DbCtx ctx,
        IDepartmentRepository deptRepo) : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger = logger;
        private readonly DbCtx _ctx = ctx;
        private readonly IDepartmentRepository _departments = deptRepo;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Department>>> GetAll(int page=1, int page_size=10)
        {
            var count = await _departments.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Department>>
            {
                Count = count,
                Value = await _departments.Get(page, page_size)
            });
        }

        [HttpGet("{uname}", Name="GetDepartmentByUName")]
        public async Task<ActionResult<Department>> Get(string uname)
        {
            return Ok(await _departments.GetByUName(uname));
        }

        [HttpGet("{uname}/doctors")]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetDoctorsOfDept(string uname, int page = 1, int page_size = 10)
        {
            if (page <= 0 || page_size <= 0 || page_size > 50)
                throw new ErrBadPagination();
            Department department = await _departments.GetByUName(uname);
            var doctors = department.Doctors.Skip((page - 1) * page_size).Take(page_size);
            int count = doctors.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IEnumerable<Doctor>> { Count = doctors.Count(), Value = doctors });
        }

        [HttpPost]
        public async Task<ActionResult<Department>> Post(DepartmentDtoNew department)
        {
            Department d = new() {
                UName = department.UName,
                Name = department.Name
            };
            await _departments.Add(d);
            return CreatedAtRoute("GetDepartmentByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DepartmentDtoPut department)
        {
            if (!await _departments.ExistsByUName(uname))
                throw new ErrNotFound();
            Department? d = new Department { UName = uname, Name = department.Name };
            await _departments.Update(d);
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            if (!await _departments.ExistsByUName(uname))
                return NotFound();
            await _departments.Delete(uname);
            return Ok();
        }
    }
}
