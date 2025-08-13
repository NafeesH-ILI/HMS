using hms.Models;
using hms.Repos;
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
        IPatientRepository patientRepo) : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger = logger;
        private readonly DbCtx _ctx = ctx;
        private readonly IPatientRepository _patients = patientRepo;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Patient>>> GetAll(int page = 1, int page_size = 10)
        {
            var count = await _patients.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Patient>>
            {
                Count = count,
                Value = await _patients.Get(page, page_size)
            });
        }

        [HttpGet("{phone}/{name}", Name = "GetPatientByPhoneName")]
        public async Task<ActionResult<Patient>> Get(string phone, string name)
        {
            return Ok(await _patients.GetByPhoneName(phone, name));
        }

        [HttpGet("{phone}")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetByPhone(string phone, int page = 1, int page_size=10)
        {
            int count = await _patients.CountByPhone(phone);
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IList<Patient>>
            {
                Count = count,
                Value = await _patients.GetByPhone(phone, page, page_size)
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
            await _patients.Add(p);
            return CreatedAtRoute("GetPatientByPhoneName", new {phone = p.Phone, name = p.Name}, p);
        }

        [HttpPut("{phone}/{name}")]
        public async Task<ActionResult> Put(string phone, string name, PatientDto patient)
        {
            Patient p = await _patients.GetByPhoneName(phone, name);
            p.Phone = patient.Phone;
            p.Name = patient.Name;
            p.DateBirth = patient.DateBirth;
            await _patients.Update(p);
            return Ok();
        }

        [HttpDelete("{phone}/{name}")]
        public async Task<ActionResult> Delete(string phone, string name)
        {
            await _patients.Delete(phone, name);
            return Ok();
        }
    }
}
