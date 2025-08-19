namespace hms.Models.DTOs
{
    public record DoctorDtoPut
    {
        public required string Name { get; set; }
        public required string MaxQualification { get; set; }
        public required string Specialization { get; set; }
        public required string DeptKey { get; set; }
    }
}
