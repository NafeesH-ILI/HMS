using hms.Models;
using hms.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/doctors")]
    [ErrorHandler]
    public class DoctorsController(
        ILogger<DoctorsController> logger,
        DbCtx ctx,
        IDoctorRepository doctorRepo) : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger = logger;
        private readonly DbCtx _ctx = ctx;
        private readonly IDoctorRepository _doctors = doctorRepo;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetAll(int page=1, int page_size=10)
        {
            var count = await _doctors.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Doctor>>
            {
                Count = count,
                Value = await _doctors.Get(page, page_size)
            });
        }

        [HttpGet("{uname}", Name="GetDoctorByUName")]
        public async Task<ActionResult<Doctor>> Get(string uname)
        {
            return Ok(await _doctors.GetByUName(uname));
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> Post(DoctorDtoNew doctor)
        {
            Doctor d = new() {
                UName = "",
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey,
            };
            await _doctors.Add(d);
            return CreatedAtRoute("GetDoctorByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DoctorDtoNew doctor)
        {
            if (!await _doctors.ExistsByUName(uname))
                return NotFound();
            Doctor d = new()
            {
                UName = uname,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey
            };
            await _doctors.Update(d);
            return Ok();
        }

        [HttpPatch("{uname}")]
        public async Task<ActionResult> Patch(string uname, DoctorDtoPatch doctor)
        {
            Doctor? d = await _ctx.Doctors.FindAsync(uname);
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
            await _doctors.Update(d);
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            if (!await _doctors.ExistsByUName(uname))
                return NotFound();
            await _doctors.Delete(uname);
            return Ok();
        }
    }
}
