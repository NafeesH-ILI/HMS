namespace hms.Models.DTOs
{
    public class AppointmentDtoGet
    {
        public required string Id { get; set; }
        public required DoctorDtoGet Doctor { get; set; }
        public required PatientDtoGet Patient { get; set; }
        public required DateTime Time { get; set; }
        public required string Status { get; set; }
    }
}
