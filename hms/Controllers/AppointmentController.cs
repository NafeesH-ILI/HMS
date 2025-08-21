using AutoMapper;
using hms.Migrations;
using hms.Models;
using hms.Models.DTOs;
using hms.Services.Interfaces;
using hms.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/v2/appointments")]
    public class AppointmentController(
        ILogger<AppointmentController> logger,
        UserManager<User> users,
        IMapper mapper,
        IAppointmentService apptService) : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger = logger;
        private readonly UserManager<User> _users = users;
        private readonly IMapper _mapper = mapper;
        private readonly IAppointmentService _apptService = apptService;

        [HttpGet]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<ActionResult<IAsyncEnumerable<AppointmentDtoGet>>> GetAll(
            [FromQuery] int page = 1, [FromQuery] int page_size = 10, [FromQuery] string? status = null,
            [FromQuery] string? patient_uname = null, [FromQuery] string? doctor_uname = null)
        {
            int count;
            Appointment.Statuses? st = status == null ? null : _mapper.Map<TypeT<Appointment.Statuses>>(
                                    new TypeTString<Appointment.Statuses> { Type = status }).Type;
            if (patient_uname != null && doctor_uname != null)
            {
                count = await _apptService.CountByDoctorPatient(doctor_uname, patient_uname, st);
                if (count == 0)
                    return NoContent();
                return Ok(new PaginatedResponse<IEnumerable<AppointmentDtoGet>>
                {
                    Count = count,
                    Value = from appt in
                                await _apptService.GetByDoctorPatient(doctor_uname, patient_uname,
                                page, page_size, status == null ? null :
                                _mapper.Map<TypeT<Appointment.Statuses>>(
                                    new TypeTString<Appointment.Statuses> { Type = status }).Type)
                            select _apptService.ToDto(appt)
                });
            }
            else if (patient_uname != null)
            {
                count = await _apptService.CountByPatient(patient_uname, st);
                if (count == 0)
                    return NoContent();
                return Ok(new PaginatedResponse<IEnumerable<AppointmentDtoGet>>
                {
                    Count = count,
                    Value = from appt in
                                await _apptService.GetByPatient(patient_uname,
                                page, page_size, status == null ? null :
                                _mapper.Map<TypeT<Appointment.Statuses>>(
                                    new TypeTString<Appointment.Statuses> { Type = status }).Type)
                            select _apptService.ToDto(appt)
                });
            }
            else if (doctor_uname != null)
            {
                count = await _apptService.CountByDoctor(doctor_uname, st);
                if (count == 0)
                    return NoContent();
                return Ok(new PaginatedResponse<IEnumerable<AppointmentDtoGet>>
                {
                    Count = count,
                    Value = from appt in
                                await _apptService.GetByDoctor(doctor_uname,
                                page, page_size, status == null ? null :
                                _mapper.Map<TypeT<Appointment.Statuses>>(
                                    new TypeTString<Appointment.Statuses> { Type = status }).Type)
                            select _apptService.ToDto(appt)
                });
            }
            else
            {
                count = await _apptService.Count();
                if (count == 0)
                    return NoContent();
                return Ok(new PaginatedResponse<IEnumerable<AppointmentDtoGet>>
                {
                    Count = count,
                    Value = from appt in
                                await _apptService.Get(page, page_size, status == null ? null :
                                _mapper.Map<TypeT<Appointment.Statuses>>(
                                    new TypeTString<Appointment.Statuses> { Type = status }).Type)
                            select _apptService.ToDto(appt)
                });
            }
        }

        [HttpGet("{id}", Name = "GetById")]
        [Authorize]
        public async Task<ActionResult<AppointmentDtoGet>> Get(string id)
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrForbidden();
            Appointment appt = await _apptService.GetById(id);
            if (user.Type != hms.Models.User.Types.Receptionist)
            {
                if (user.Id != appt.DoctorId && user.Id != appt.PatientId)
                    throw new ErrNotLegal();
            }
            return Ok(_apptService.ToDto(appt));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Receptionist)]
        public async Task<ActionResult<AppointmentDtoGet>> Post([FromBody] AppointmentDtoNew dto)
        {
            AppointmentDtoGet res = _apptService.ToDto(await _apptService.Add(dto));
            return CreatedAtRoute("GetById", new {id =  res.Id}, res);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> Patch(string id, [FromBody] AppointmentDtoPatch dto)
        {
            User user = await _users.GetUserAsync(User) ?? throw new ErrForbidden();
            Appointment appt = await _apptService.GetById(id);
            if (dto.Time != null)
            {
                // do not let patient change time
                if (user.Type == hms.Models.User.Types.Patient)
                    throw new ErrForbidden("You cannot change appointment time");
            }
            if (dto.Status != null)
            {
                Appointment.Statuses status = _mapper.Map<TypeT<Appointment.Statuses>>(
                                        new TypeTString<Appointment.Statuses> { Type = dto.Status }).Type;
                // only doctor can mark as Done
                if (status == Appointment.Statuses.Done &&
                    (user.Type != hms.Models.User.Types.Doctor &&
                    user.Type != hms.Models.User.Types.Admin &&
                    user.Type != hms.Models.User.Types.SuperAdmin))
                    throw new ErrForbidden("You cannot mark appointment as done");

                // only doctor/patient/receptionist can cancel
                if (status == Appointment.Statuses.Cancelled)
                {
                    if (user.Type == hms.Models.User.Types.Doctor)
                    {
                        if (appt.DoctorId != user.Id)
                            throw new ErrForbidden("You cannot change this appointment's status");
                    }
                    else if (user.Type == hms.Models.User.Types.Patient)
                    {
                        if (appt.PatientId != user.Id)
                            throw new ErrForbidden("You cannot change this appointment's status");
                    }
                    else if (user.Type != hms.Models.User.Types.Receptionist &&
                        user.Type != hms.Models.User.Types.Admin &&
                        user.Type != hms.Models.User.Types.SuperAdmin)
                        throw new ErrForbidden("You cannot change this appointment's status");
                }
            }
            appt = await _apptService.Update(Guid.Parse(id), dto);
            return Ok(_apptService.ToDto(appt));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task Delete(string id)
        {
            await _apptService.Delete(Guid.Parse(id));
        }
    }
}
