using hms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/departments")]
    public class DepartmentsController(ILogger<DepartmentsController> logger) : ControllerBase
    {
        private readonly ILogger<DepartmentsController> logger = logger;
        private readonly DbCtx ctx = new ();

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Department>>> GetAll(int page=0, int page_size=10)
        {
            if (page <= 0)
                page = 1;
            if (page_size <= 0 || page_size > 50)
                page_size = 10;
            var res = await ctx.Departments
                .OrderBy(d => d.UName)
                .Skip((page - 1) * page_size)
                .Take(page_size)
                .ToListAsync();
            if (res.Count == 0)
                return NoContent();
            return Ok(res);
        }

        [HttpGet("{uname}", Name="GetDepartmentByUName")]
        public async Task<ActionResult<Department>> Get(string uname)
        {
            Department? department;
            try
            {
                department = await ctx.Departments.FindAsync(uname);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Get Department with id={0}", uname);
                return BadRequest();
            }
            if (department == null)
                return NotFound();
            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<Department>> Post(DepartmentDtoNew department)
        {
            Department d = new() {
                UName = department.UName,
                Name = department.Name
            };
            try
            {
                ctx.Departments.Add(d);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Post department");
                return BadRequest();
            }
            return CreatedAtRoute("GetDepartmentByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DepartmentDtoPut department)
        {
            Department? d = await ctx.Departments.FindAsync(uname);
            if (d == null)
                return NotFound();
            d.Name = department.Name;
            try
            {
                ctx.Departments.Update(d);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Put department");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            Department? d = await ctx.Departments.FindAsync(uname);
            if (d == null)
                return NotFound();
            try
            {
                ctx.Departments.Remove(d);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Delete Department");
                return BadRequest();
            }
            return Ok();
        }
    }
}
