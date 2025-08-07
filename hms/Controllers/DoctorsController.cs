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
        public IAsyncEnumerable<Doctor> GetAll()
        {
            return Ctx.Doctors.AsAsyncEnumerable();
        }

        [HttpGet("{id}", Name="GetDoctorById")]
        public async Task<ActionResult<Doctor>> Get(int id)
        {
            Doctor? doctor = await Ctx.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> Post(DoctorDTOPost doctor)
        {
            Doctor d = new() {
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization
            };
            Ctx.Doctors.Add(d);
            await Ctx.SaveChangesAsync();
            return CreatedAtRoute("GetDoctorById", new { id = d.Id }, d);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, DoctorDTOPost doctor)
        {
            if (await Ctx.Doctors.FindAsync(id) == null)
                return NotFound();
            Doctor d = new() {
                Id = id,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization
            };
            Ctx.Doctors.Update(d);
            await Ctx.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, DoctorDTOPut doctor)
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
            Ctx.Doctors.Update(d);
            await Ctx.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Doctor? d = await Ctx.Doctors.FindAsync(id);
            if (d == null)
                return NotFound();
            Ctx.Doctors.Remove(d);
            await Ctx.SaveChangesAsync();
            return Ok();
        }
    }
}
