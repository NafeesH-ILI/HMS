using hms.Models;
using hms.Repos;
using hms.Services;
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
        IDoctorService doctorService) : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger = logger;
        private readonly DbCtx _ctx = ctx;
        private readonly IDoctorService _doctorService = doctorService;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetAll(int page=1, int page_size=10)
        {
            var count = await _doctorService.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Doctor>>
            {
                Count = count,
                Value = await _doctorService.Get(page, page_size)
            });
        }

        [HttpGet("{uname}", Name="GetDoctorByUName")]
        public async Task<ActionResult<Doctor>> Get(string uname)
        {
            return Ok(await _doctorService.GetByUName(uname));
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
            await _doctorService.Add(d);
            return CreatedAtRoute("GetDoctorByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DoctorDtoNew doctor)
        {
            if (!await _doctorService.ExistsByUName(uname))
                return NotFound();
            Doctor d = new()
            {
                UName = uname,
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey
            };
            await _doctorService.Update(d);
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
            await _doctorService.Update(d);
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            if (!await _doctorService.ExistsByUName(uname))
                return NotFound();
            await _doctorService.Delete(uname);
            return Ok();
        }
    }
}
