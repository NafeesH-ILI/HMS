namespace hms.Models.DTOs
{
    public class AppointmentDtoGet
    {
        public required string Id { get; set; }
        public required string DoctorUName { get; set; }
        public required string PatientUName { get; set; }
        public required DateTime Time { get; set; }
        public required string Status { get; set; }
    }
}
