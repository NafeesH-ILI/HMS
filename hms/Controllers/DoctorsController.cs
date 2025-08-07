using Microsoft.AspNetCore.Mvc;

using hms.Models;
using Microsoft.EntityFrameworkCore;

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
        public ActionResult<IAsyncEnumerable<Doctor>> GetAll(int? after=0)
        {
            int lastId = 0;
            if (after != null)
                lastId = after.Value;
            Logger.LogInformation("lastId = " + lastId);
            var res = Ctx.Doctors
                .OrderBy(d => d.Id)
                .Where(d => d.Id > lastId)
                .Take(10)
                .AsAsyncEnumerable();
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
        public async Task<ActionResult<Doctor>> Post(DoctorDtoPost doctor)
        {
            Doctor d = new() {
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization
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
        public async Task<ActionResult> Put(int id, DoctorDtoPost doctor)
        {
            if (await Ctx.Doctors.FindAsync(id) == null)
                return NotFound();
            Doctor d = new() {
                Id = id,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization
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
        public async Task<ActionResult> Patch(int id, DoctorDtoPut doctor)
        {
            Doctor? d = new() { Id = id };
            d = await Ctx.Doctors.FindAsync(id);
            if (d == null)
                return NotFound();
            if (doctor.Name != null)
                d.Name = doctor.Name;
            if (doctor.Specialization != null)
                d.Specialization = doctor.Specialization;
            if (doctor.MaxQualification != null)
                d.MaxQualification = doctor.MaxQualification;
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
