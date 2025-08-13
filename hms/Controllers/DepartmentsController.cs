using hms.Models;
using hms.Repos;
using hms.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/departments")]
    [ErrorHandler]
    public class DepartmentsController(
        ILogger<DepartmentsController> logger,
        IDepartmentService deptService) : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger = logger;
        private readonly IDepartmentService _deptService = deptService;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Department>>> GetAll(int page=1, int page_size=10)
        {
            var count = await _deptService.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Department>>
            {
                Count = count,
                Value = await _deptService.Get(page, page_size)
            });
        }

        [HttpGet("{uname}", Name="GetDepartmentByUName")]
        public async Task<ActionResult<Department>> Get(string uname)
        {
            return Ok(await _deptService.GetByUName(uname));
        }

        [HttpGet("{uname}/doctors")]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetDoctorsOfDept(string uname, int page = 1, int page_size = 10)
        {
            Department department = await _deptService.GetByUName(uname);
            var doctors = department.Doctors.Skip((page - 1) * page_size).Take(page_size);
            int count = doctors.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IEnumerable<Doctor>> { Count = doctors.Count(), Value = doctors });
        }

        [HttpPost]
        public async Task<ActionResult<Department>> Post(DepartmentDtoNew department)
        {
            Department d = await _deptService.Add(department);
            return CreatedAtRoute("GetDepartmentByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DepartmentDtoPut department)
        {
            await _deptService.Update(uname, department);
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            await _deptService.Delete(uname);
            return Ok();
        }
    }
}
