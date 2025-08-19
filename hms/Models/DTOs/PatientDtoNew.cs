namespace hms.Models.DTOs
{
    public record PatientDtoNew
    {
        public required string Phone { get; set; }
        public required string Name { get; set; }
        public required DateOnly DateBirth { get; set; }
        public string? Password { get; set; }
    }
}
