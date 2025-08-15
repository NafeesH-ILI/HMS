using hms.Models;
using hms.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/doctors")]
    [ErrorHandler]
    public class DoctorsController(
        ILogger<DoctorsController> logger,
        IDoctorService doctorService) : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger = logger;
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
            Doctor d = await _doctorService.Add(doctor);
            return CreatedAtRoute("GetDoctorByUName", new { uname = d.UName }, d);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DoctorDtoNew doctor)
        {
            await _doctorService.Update(uname, doctor);
            return Ok();
        }

        [HttpPatch("{uname}")]
        public async Task<ActionResult> Patch(string uname, DoctorDtoPatch doctor)
        {
            await _doctorService.Update(uname, doctor);
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            await _doctorService.Delete(uname);
            return Ok();
        }
    }
}
