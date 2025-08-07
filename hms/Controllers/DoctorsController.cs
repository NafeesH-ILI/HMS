using Microsoft.AspNetCore.Mvc;

using hms.Models;
using Microsoft.EntityFrameworkCore;

namespace hms.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public IAsyncEnumerable<Doctor> GetAll()
        {
            return Ctx.Doctors.AsAsyncEnumerable();
        }

        [HttpGet("{id}", Name="GetDoctorById")]
        public async Task<ActionResult<Doctor>> Get(int id)
        {
            Doctor? doctor = await Ctx.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> Post(Doctor_NoId doctor)
        {
            Doctor d = new() { Name = doctor.Name };
            try
            {
                Ctx.Doctors.Add(d);
                await Ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError("Post Doctor: " + ex.Message);
                return StatusCode(500);
            }
            return CreatedAtRoute("GetDoctorById", new { id = d.Id }, doctor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Doctor>> Put(int id, Doctor_NoId doctor)
        {
            Doctor d = new() { Id = id, Name = doctor.Name };
            try
            {
                Ctx.Doctors.Update(d);
                if (await Ctx.SaveChangesAsync() <= 0)
                {
                    return NotFound(d);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Doctor Put: " + ex.Message);
                return StatusCode(500);
            }
            return d;
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Doctor>> Patch(int id, Doctor_NoId_Optional doctor)
        {
            // TODO: implement this
            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Doctor d = new() { Id = id };
                Ctx.Doctors.Remove(d);
                if (await Ctx.SaveChangesAsync() <= 0)
                {
                    return NotFound(d);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Doctor Delete: " + ex.Message);
                return StatusCode(500);
            }
            return Ok();
        }
    }
}
