using hms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace hms.Controllers
{
    [ApiController]
    [Route("doctors")]
    public class DoctorsController : ControllerBase
    {
        private ILogger<DoctorsController> Logger;
        private DbCtx Ctx;
        public DoctorsController(ILogger<DoctorsController> logger)
        {
            Ctx = new DbCtx();
            Logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetAll(int? page=1, int? page_size=10)
        {
            /*int lastId = 0;
            if (after != null)
                lastId = after.Value;
            var res = Ctx.Doctors
                .OrderBy(d => d.Id)
                .Where(d => d.Id > lastId)
                .Take(10)
                .AsAsyncEnumerable();*/
            if (page <= 0)
                page = 1;
            if (page_size <= 0 || page_size > 50)
                page_size = 10;
            var res = await Ctx.Doctors
                .FromSql($"SELECT * FROM doctors ORDER BY id LIMIT {page_size} OFFSET {(page - 1) * page_size};")
                .ToListAsync();
            return Ok(res);
        }

        [HttpGet("{id}", Name="GetDoctorById")]
        public async Task<ActionResult<Doctor>> Get(int id)
        {
            Doctor? doctor;
            try
            {
                doctor = await Ctx.Doctors.FindAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Get doctor with id={0}", id);
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
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                //DeptId = doctor.DeptId,
            };
            try
            {
                Ctx.Doctors.Add(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Post doctor");
                return BadRequest();
            }
            return CreatedAtRoute("GetDoctorById", new { id = d.Id }, d);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, DoctorDtoNew doctor)
        {
            if (await Ctx.Doctors.FindAsync(id) == null)
                return NotFound();
            Doctor d = new()
            {
                Id = id,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                //DeptId = doctor.DeptId
            };
            try
            {
                Ctx.Doctors.Update(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Put doctor");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, DoctorDtoPatch doctor)
        {
            Doctor? d = await Ctx.Doctors.FindAsync(id);
            if (d == null)
                return NotFound();
            if (doctor.Name != null)
                d.Name = doctor.Name;
            if (doctor.Specialization != null)
                d.Specialization = doctor.Specialization;
            if (doctor.MaxQualification != null)
                d.MaxQualification = doctor.MaxQualification;
            //if (doctor.DeptId != null)
            //    d.DeptId = doctor.DeptId;
            try
            {
                Ctx.Doctors.Update(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Faield to Patch Doctor");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Doctor? d = await Ctx.Doctors.FindAsync(id);
            if (d == null)
                return NotFound();
            try
            {
                Ctx.Doctors.Remove(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Delete Doctor");
                return BadRequest();
            }
            return Ok();
        }
    }
}
