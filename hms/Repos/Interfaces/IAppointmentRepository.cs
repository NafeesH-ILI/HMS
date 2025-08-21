using hms.Models;
using hms.Models.DTOs;

namespace hms.Repos.Interfaces
{
    public interface IAppointmentRepository
    {
        public Task<Appointment?> GetById(Guid Id);
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
            Appointment.Statuses? status = null);
        public Task<IList<Appointment>> GetByDoctorPatient(string doctorUName, string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null);
        public Task Add(Appointment appt);
        public Task Update(Appointment appt);
        public Task Delete(Appointment appt);
    }
}
