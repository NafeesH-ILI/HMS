using hms.Models;
using hms.Models.DTOs;

namespace hms.Services.Interfaces
{
    public interface IAppointmentService
    {
        public Task<int> Count();
        public Task<IList<Appointment>> Get(int page = 1, int pageSize = 10,
            Appointment.Statuses? status = null);

        public Task<int> CountByPatient(string patientUName, Appointment.Statuses? status = null);

        public Task<IList<Appointment>> GetByPatient(string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null);

        public Task<int> CountByDoctor(string doctorUName, Appointment.Statuses? status = null);

        public Task<IList<Appointment>> GetByDoctor(string doctorUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null);

        public Task<int> CountByDoctorPatient(string doctorUName, string patientUName,
            Appointment.Statuses? status);

        public Task<IList<Appointment>> GetByDoctorPatient(string doctorUName, string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null);

        public Task<Appointment> Add(AppointmentDtoNew dto);

        public Task<Appointment> Update(Guid Id, AppointmentDtoPut dto);

        public Task Delete(Guid Id);
    }
}
