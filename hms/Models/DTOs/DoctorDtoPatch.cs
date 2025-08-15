namespace hms.Models.DTOs
{
    public record DoctorDtoPatch
    {
        public string? Name { get; set; }
        public string? MaxQualification { get; set; }
        public string? Specialization { get; set; }
        public string? DeptKey { get; set; }
    }
}
