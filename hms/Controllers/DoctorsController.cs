using AutoMapper;
using hms.Models;
using hms.Models.DTOs;
using hms.Services.Interfaces;
using hms.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/doctors")]
    [Authorize]
    public class DoctorsController(
        ILogger<DoctorsController> logger,
        IDoctorService doctorService,
        IMapper mapper) : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger = logger;
        private readonly IDoctorService _doctorService = doctorService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Roles = Roles.Anyone)]
        public async Task<ActionResult<IEnumerable<DoctorDtoGet>>> GetAll(int page=1, int page_size=10)
        {
            var count = await _doctorService.Count();
            if (count == 0)
                return NoContent();
            return Ok(new PaginatedResponse<IEnumerable<DoctorDtoGet>>
            {
                Count = count,
                Value = (await _doctorService.Get(page, page_size)).Select(d => _doctorService.ToDtoGet(d))
            });
        }

        [HttpGet("{uname}", Name="GetDoctorByUName")]
        [Authorize(Roles = Roles.Anyone)]
        public async Task<ActionResult<DoctorDtoGet>> GetByUName(string uname)
        {
            return Ok(_doctorService.ToDtoGet(await _doctorService.GetByUName(uname)));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<DoctorDtoGet>> Post(DoctorDtoNew doctor)
        {
            Doctor d = await _doctorService.Add(doctor);
            DoctorDtoGet dto = _doctorService.ToDtoGet(d);
            return CreatedAtRoute("GetDoctorByUName", new { uname = d.User.UserName }, dto);
        }

        [HttpPut("{uname}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> Put(string uname, DoctorDtoPut doctor)
        {
            await _doctorService.Update(uname, doctor);
            return Ok();
        }

        [HttpPatch("{uname}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> Patch(string uname, DoctorDtoPatch doctor)
        {
            await _doctorService.Update(uname, doctor);
            return Ok();
        }

        [HttpDelete("{uname}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> Delete(string uname)
        {
            await _doctorService.Delete(uname);
            return Ok();
        }
    }
}
