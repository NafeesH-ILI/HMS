using hms.Models;
using hms.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/patients")]
    [ErrorHandler]
    public class PatientsController(
        ILogger<PatientsController> logger,
        IPatientService patientService) : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger = logger;
        private readonly IPatientService _patientService = patientService;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Patient>>> GetAll(int page = 1, int page_size = 10)
        {
            var count = await _patientService.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Patient>>
            {
                Count = count,
                Value = await _patientService.Get(page, page_size)
            });
        }

        [HttpGet("{uname}", Name = "GetPatientByUName")]
        public async Task<ActionResult<Patient>> Get(string uname)
        {
            return Ok(await _patientService.GetByUName(uname));
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> Post(PatientDtoNew patientDto)
        {
            Patient patient = await _patientService.Add(patientDto);
            return CreatedAtRoute("GetPatientByUName",
                new {uname = patient.UName},
                patient);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, PatientDtoNew patient)
        {
            await _patientService.Update(uname, patient);
            return Ok();
        }

        [HttpPatch("{uname}")]
        public async Task<ActionResult> Put(string uname, PatientDtoPatch patient)
        {
            await _patientService.Update(uname, patient);
            return Ok();
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            await _patientService.Delete(uname);
            return Ok();
        }
    }
}
