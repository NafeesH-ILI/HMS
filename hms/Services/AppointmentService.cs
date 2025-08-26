using AutoMapper;
using hms.Utils;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos.Interfaces;
using hms.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace hms.Services
{
    public class AppointmentService(
        IMapper mapper,
        IDoctorService doctorService,
        IPatientService patientService,
        IUserService usersService,
        IAppointmentRepository apptRepo) : IAppointmentService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IDoctorService _doctorService = doctorService;
        private readonly IPatientService _patientService = patientService;
        private readonly IUserService _userService = usersService;
        private readonly IAppointmentRepository _appts = apptRepo;

        public async Task<Appointment> GetById(string id)
        {
            return await _appts.GetById(Guid.Parse(id)) ?? throw new ErrNotFound("Appointment Not Found");
        }

        public async Task<int> Count(Appointment.Statuses? status = null)
        {
            return await _appts.Count(status);
        }

        public async Task<IList<Appointment>> Get(int page = 1, int pageSize = 10,
            Appointment.Statuses? status = null)
        {
            return await _appts.Get(page, pageSize, status);
        }

        public async Task<int> CountByPatient(string patientUName, Appointment.Statuses? status = null)
        {
            return await _appts.CountByPatient(patientUName, status);
        }

        public async Task<IList<Appointment>> GetByPatient(string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null)
        {
            return await _appts.GetByPatient(patientUName, page, pageSize, status);
        }

        public async Task<int> CountByDoctor(string doctorUName, Appointment.Statuses? status = null)
        {
            return await _appts.CountByDoctor(doctorUName, status);
        }

        public async Task<IList<Appointment>> GetByDoctor(string doctorUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null)
        {
            return await _appts.GetByDoctor(doctorUName, page, pageSize, status);
        }

        public async Task<int> CountByDoctorPatient(string doctorUName, string patientUName,
            Appointment.Statuses? status)
        {
            return await _appts.CountByDoctorPatient(doctorUName, patientUName, status);
        }

        public async Task<IList<Appointment>> GetByDoctorPatient(string doctorUName, string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null)
        {
            return await _appts.GetByDoctorPatient(doctorUName, patientUName, page, pageSize, status);
        }

        public async Task<Appointment> Add(AppointmentDtoNew dto)
        {
            Doctor doctor = await _doctorService.GetByUName(dto.DoctorUName);
            Patient patient = await _patientService.GetByUName(dto.PatientUName);
            dto.Time = new DateTime(dto.Time.Year, dto.Time.Month, dto.Time.Day,
                                        dto.Time.Hour, dto.Time.Minute, dto.Time.Second,
                                        DateTimeKind.Unspecified);
            if (dto.Time < DateTime.Now)
                throw new ErrBadReq("Cannot schedule Appointment in the past");
            Appointment appt = new()
            {
                Id = Guid.NewGuid(),
                DoctorId = doctor.Id,
                PatientId = patient.Id,
                Status = Appointment.Statuses.Scheduled,
                Time = dto.Time
            };
            await _appts.Add(appt);
            return appt;
        }

        public async Task<Appointment> Update(Guid Id, AppointmentDtoPatch dto)
        {
            if (dto.Status != null && dto.Time != null)
                throw new ErrBadReq("Cannot update both Time and Status at once");
            Appointment? appt = await _appts.GetById(Id) ?? throw new ErrNotFound();
            if (appt.Status == Appointment.Statuses.Done)
                throw new ErrBadReq("Cannot update an Appointment with Done status");
            if (appt.Status == Appointment.Statuses.Cancelled)
                throw new ErrBadReq("Cannot update a Cancelled Appointment");
            if (dto.Status != null)
            {
                appt.Status = _mapper.Map<TypeT<Appointment.Statuses>>(
                    new TypeTString<Appointment.Statuses> { Type = dto.Status }).Type;
            }
            if (dto.Time != null)
            {
                if (dto.Time < DateTime.Now)
                    throw new ErrBadReq("Cannot schedule Appointment in the past");
                appt.Time = (DateTime)dto.Time;
            }
            await _appts.Update(appt);
            return appt;
        }

        public async Task Delete(Guid Id)
        {
            await _appts.Delete(await _appts.GetById(Id) ?? throw new ErrNotFound("Appointment Not Found"));
        }

        public AppointmentDtoGet ToDto(Appointment appt)
        {
            AppointmentDtoGet dto = new()
            {
                Id = appt.Id.ToString(),
                DoctorUName = "",
                PatientUName = "",
                Time = appt.Time,
                Status = _mapper.Map<TypeTString<Appointment.Statuses>>(
                    new TypeT<Appointment.Statuses> { Type = appt.Status }).Type,
            };
            if (appt.DoctorUser == null)
                dto.DoctorUName = _userService.UNameOf(appt.DoctorId) ??
                    throw new ErrNotFound("Doctor Not Found");
            else
                dto.DoctorUName = appt.DoctorUser.UserName!;
            if (appt.PatientUser == null)
                dto.PatientUName = _userService.UNameOf(appt.PatientId) ??
                    throw new ErrNotFound("Patient Not Found");
            else
                dto.PatientUName = appt.PatientUser.UserName!;
            return dto;
        }

        public async Task AutoCancel()
        {
            await _appts.AutoCancel(DateTime.Now.AddMinutes(-Consts.ApptAutoCancelMinutes));
        }
    }
}

