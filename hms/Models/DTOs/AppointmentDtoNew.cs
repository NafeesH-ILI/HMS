namespace hms.Models.DTOs
{
    public class AppointmentDtoNew
    {
        public required string DoctorUName { get; set; }
        public required string PatientUName { get; set; }
        public required DateTime Time { get; set; }
    }
}
