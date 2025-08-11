using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("doctors")]
    public class Doctor
    {
        [Column("uname")]
        [Key]
        public required string UName { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("max_qual")]
        public string? MaxQualification { get; set; }

        [Column("specialization")]
        public string? Specialization { get; set; }

        //[ForeignKey(nameof(Department))]
        [Column("dept")]
        public string? DeptKey { get; set; }

        //public Department Dept { get; set; } = null!;
    }

    public record DoctorsPaginated
    {
        public required int Count { get; set; }
        public required IEnumerable<Doctor> Doctors { get; set; }
    } 

    public record DoctorDtoNew
    {
        public required string Name { get; set; }
        public required string MaxQualification { get; set; }
        public required string Specialization { get; set; }
        public required string DeptKey { get; set; }
    }
    public record DoctorDtoPatch
    {
        public string? Name { get; set; }
        public string? MaxQualification { get; set; }
        public string? Specialization { get; set; }
        public string? DeptKey { get; set; }
    }
}
