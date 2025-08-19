namespace hms.Models.DTOs
{
    public record PatientDtoPut
    {
        public required string Phone { get; set; }
        public required string Name { get; set; }
        public required DateOnly DateBirth { get; set; }
    }
}
