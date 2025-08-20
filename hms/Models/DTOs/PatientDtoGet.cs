namespace hms.Models.DTOs
{
    public class PatientDtoGet
    {
        public required string UName { get; set; }
        public required string Phone { get; set; }
        public required string Name { get; set; }
        public required DateOnly DateBirth { get; set; }
    }
}
