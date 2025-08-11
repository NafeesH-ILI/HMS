using hms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/doctors")]
    public class DoctorsController(ILogger<DoctorsController> logger) : ControllerBase
    {
        private readonly ILogger<DoctorsController> logger = logger;
        private readonly DbCtx ctx = new ();

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetAll(int page=1, int page_size=10)
        {
            if (page <= 0)
                page = 1;
            if (page_size <= 0 || page_size > 50)
                page_size = 10;
            var res = await ctx.Doctors
                .OrderBy(d => d.UName)
                //.Include(d => d.Dept) // would make payload too heavy
                .Skip((page - 1) * page_size)
                .Take(page_size)
                .ToListAsync();
            var Count = await ctx.Doctors.CountAsync();
            if (Count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<List<Doctor>> { Count = Count, Value = res});
        }

        [HttpGet("{uname}", Name="GetDoctorByUName")]
        public async Task<ActionResult<Doctor>> Get(string uname)
        {
            Doctor? doctor;
            try
            {
                doctor = ctx.Doctors
                    .Where(d => d.UName == uname)
                    .Include(d => d.Dept)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Get doctor with uname={0}", uname);
                return BadRequest();
            }
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> Post(DoctorDtoNew doctor)
        {
            Doctor d = new() {
                UName = UNamer.Generate("doctors", doctor.Name),
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey,
            };
            try
            {
                ctx.Doctors.Add(d);
                await ctx.SaveChangesAsync();
                d.Dept = (await ctx.Departments.FindAsync(d.DeptKey))!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Post doctor");
                return BadRequest();
            }
            return CreatedAtRoute("GetDoctorByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DoctorDtoNew doctor)
        {
            if (await ctx.Doctors.CountAsync(d => d.UName == uname) == 0)
                return NotFound();
            Doctor d = new()
            {
                UName = uname,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey
            };
            try
            {
                ctx.Doctors.Update(d);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Put doctor");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{uname}")]
        public async Task<ActionResult> Patch(string uname, DoctorDtoPatch doctor)
        {
            Doctor? d = await ctx.Doctors.FindAsync(uname);
            if (d == null)
                return NotFound();
            if (doctor.Name != null)
                d.Name = doctor.Name;
            if (doctor.Specialization != null)
                d.Specialization = doctor.Specialization;
            if (doctor.MaxQualification != null)
                d.MaxQualification = doctor.MaxQualification;
            if (doctor.DeptKey != null)
                d.DeptKey = doctor.DeptKey;
            try
            {
                ctx.Doctors.Update(d);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Faield to Patch Doctor");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Doctor? d = await ctx.Doctors.FindAsync(id);
            if (d == null)
                return NotFound();
            try
            {
                ctx.Doctors.Remove(d);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Delete Doctor");
                return BadRequest();
            }
            return Ok();
        }
    }
}
