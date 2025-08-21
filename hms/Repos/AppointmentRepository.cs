using hms.Migrations;
using hms.Models;
using hms.Repos.Interfaces;
using hms.Utils;
using Microsoft.EntityFrameworkCore;

namespace hms.Repos
{
    public class AppointmentRepository(
        DbCtx ctx) : IAppointmentRepository
    {
        private readonly DbCtx _ctx = ctx;

        public async Task<Appointment?> GetById(Guid Id)
        {
            return await _ctx.Appointments.FindAsync(Id);
        }

        public async Task<int> Count()
        {
            return await _ctx.Appointments.CountAsync();
        }
        public async Task<IList<Appointment>> Get(int page = 1, int pageSize = 10,
            Appointment.Statuses? status = null)
        {
            if (!Pagination.IsValid(page, pageSize)) throw new ErrBadPagination();
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByPatient(string patientUName, Appointment.Statuses? status = null)
        {
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Include(a => a.PatientUser)
                .Where(a => a.PatientUser!.UserName == patientUName)
                .CountAsync();
        }
        public async Task<IList<Appointment>> GetByPatient(string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null)
        {
            if (!Pagination.IsValid(page, pageSize)) throw new ErrBadPagination();
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Include(a => a.PatientUser)
                .Where(a => a.PatientUser!.UserName == patientUName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByDoctor(string doctorUName, Appointment.Statuses? status = null)
        {
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Include(a => a.DoctorUser)
                .Where(a => a.DoctorUser!.UserName == doctorUName)
                .CountAsync();
        }
        public async Task<IList<Appointment>> GetByDoctor(string doctorUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null)
        {
            if (!Pagination.IsValid(page, pageSize)) throw new ErrBadPagination();
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Include(a => a.PatientUser)
                .Where(a => a.PatientUser!.UserName == doctorUName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByDoctorPatient(string doctorUName, string patientUName,
            Appointment.Statuses? status = null)
        {
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Include(a => a.DoctorUser)
                .Include(a => a.PatientUser)
                .Where(a => a.DoctorUser!.UserName == doctorUName)
                .Where(a => a.PatientUser!.UserName == patientUName)
                .CountAsync();
        }
        public async Task<IList<Appointment>> GetByDoctorPatient(string doctorUName, string patientUName,
            int page = 1, int pageSize = 10, Appointment.Statuses? status = null)
        {
            if (!Pagination.IsValid(page, pageSize)) throw new ErrBadPagination();
            return await _ctx.Appointments
                .Where(a => status == null || a.Status == status)
                .Include(a => a.DoctorUser)
                .Include(a => a.PatientUser)
                .Where(a => a.DoctorUser!.UserName == doctorUName)
                .Where(a => a.PatientUser!.UserName == patientUName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Add(Appointment appt)
        {
            _ctx.Appointments.Add(appt);
            await _ctx.SaveChangesAsync();
        }

        public async Task Update(Appointment appt)
        {
            _ctx.Appointments.Update(appt);
            await _ctx.SaveChangesAsync();
        }

        public async Task Delete(Appointment appt)
        {
            _ctx.Appointments.Remove(appt);
            await _ctx.SaveChangesAsync();
        }
    }
}
