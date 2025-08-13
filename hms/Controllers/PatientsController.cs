using hms.Models;
using hms.Repos;
using hms.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/patients")]
    [ErrorHandler]
    public class PatientsController(
        ILogger<PatientsController> logger,
        DbCtx ctx,
        IPatientService patientService) : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger = logger;
        private readonly DbCtx _ctx = ctx;
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

        [HttpGet("{phone}/{name}", Name = "GetPatientByPhoneName")]
        public async Task<ActionResult<Patient>> Get(string phone, string name)
        {
            return Ok(await _patientService.GetByPhoneName(phone, name));
        }

        [HttpGet("{phone}")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetByPhone(string phone, int page = 1, int page_size=10)
        {
            int count = await _patientService.CountByPhone(phone);
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Patient>>
            {
                Count = count,
                Value = await _patientService.GetByPhone(phone, page, page_size)
            });
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> Post(PatientDto patient)
        {
            Patient p = new()
            {
                Phone = patient.Phone,
                Name = patient.Name,
                DateBirth = patient.DateBirth
            };
            await _patientService.Add(p);
            return CreatedAtRoute("GetPatientByPhoneName", new {phone = p.Phone, name = p.Name}, p);
        }

        [HttpPut("{phone}/{name}")]
        public async Task<ActionResult> Put(string phone, string name, PatientDto patient)
        {
            Patient p = await _patientService.GetByPhoneName(phone, name);
            p.Phone = patient.Phone;
            p.Name = patient.Name;
            p.DateBirth = patient.DateBirth;
            await _patientService.Update(p);
            return Ok();
        }

        [HttpDelete("{phone}/{name}")]
        public async Task<ActionResult> Delete(string phone, string name)
        {
            await _patientService.Delete(phone, name);
            return Ok();
        }
    }
}
