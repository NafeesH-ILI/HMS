using hms.Models;
using hms.Models.DTOs;
using hms.Services.Interfaces;
using hms.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/patients")]
    [Authorize]
    public class PatientsController(
        ILogger<PatientsController> logger,
        IPatientService patientService) : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger = logger;
        private readonly IPatientService _patientService = patientService;

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<IEnumerable<PatientDtoGet>>> GetAll(int page = 1, int page_size = 10)
        {
            var count = await _patientService.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IEnumerable<PatientDtoGet>>
            {
                Count = count,
                Value = (await _patientService.Get(page, page_size)).Select(p => _patientService.ToDtoGet(p))
            });
        }

        [HttpGet("{uname}", Name = "GetPatientByUName")]
        [Authorize(Roles = Roles.AnyoneButPatient)]
        public async Task<ActionResult<PatientDtoGet>> Get(string uname)
        {
            return Ok(_patientService.ToDtoGet(await _patientService.GetByUName(uname)));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<ActionResult<PatientDtoGet>> Post(PatientDtoNew patientDto)
        {
            PatientDtoGet patient = _patientService.ToDtoGet(await _patientService.Add(patientDto));
            return CreatedAtRoute("GetPatientByUName",
                new {uname = patient.UName},
                patient);
        }

        [HttpPut("{uname}")]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<ActionResult> Put(string uname, PatientDtoPut patient)
        {
            await _patientService.Update(uname, patient);
            return Ok();
        }

        [HttpPatch("{uname}")]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<ActionResult> Patch(string uname, PatientDtoPatch patient)
        {
            await _patientService.Update(uname, patient);
            return Ok();
        }

        [HttpDelete("{uname}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<ActionResult> Delete(string uname)
        {
            await _patientService.Delete(uname);
            return Ok();
        }
    }
}
