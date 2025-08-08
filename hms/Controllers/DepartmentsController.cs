using hms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hms.Controllers
{
    [ApiController]
    [Route("departments")]
    public class DepartmentsController : ControllerBase
    {
        private ILogger<DepartmentsController> Logger;
        private DbCtx Ctx;

        public DepartmentsController(ILogger<DepartmentsController> logger)
        {
            this.Logger = logger;
            this.Ctx = new DbCtx();
        }

        [HttpGet]
        public ActionResult<IAsyncEnumerable<Department>> GetAll(int? after=0)
        {
            int lastId = 0;
            if (after != null)
                lastId = after.Value;
            var res = Ctx.Departments
                .OrderBy(d => d.Id)
                .Where(d => d.Id > lastId)
                .Take(10)
                .AsAsyncEnumerable();
            return Ok(res);
        }

        [HttpGet("{id}", Name="GetDepartmentById")]
        public async Task<ActionResult<Department>> Get(int id)
        {
            Department? department;
            try
            {
                department = await Ctx.Departments.FindAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Get Department with id={0}", id);
                return BadRequest();
            }
            if (department == null)
                return NotFound();
            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<Department>> Post(DepartmentDtoNew department)
        {
            Department d = new() { Name = department.Name };
            try
            {
                Ctx.Departments.Add(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Post department");
                return BadRequest();
            }
            return CreatedAtRoute("GetDepartmentById", new { id = d.Id }, d);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, DepartmentDtoNew department)
        {
            if (await Ctx.Departments.FindAsync(id) == null)
                return NotFound();
            Department d = new() { Id = id, Name = department.Name };
            try
            {
                Ctx.Departments.Update(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Put department");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Department? d = await Ctx.Departments.FindAsync(id);
            if (d == null)
                return NotFound();
            try
            {
                Ctx.Departments.Remove(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Delete Department");
                return BadRequest();
            }
            return Ok();
        }
    }
}
