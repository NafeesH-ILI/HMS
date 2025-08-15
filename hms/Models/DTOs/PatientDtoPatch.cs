namespace hms.Models.DTOs
{
    public record PatientDtoPatch
    {
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public DateOnly? DateBirth { get; set; }
    }
}
