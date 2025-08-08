using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("doctors")]
    public class Doctor
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("max_qual")]
        public string? MaxQualification { get; set; }

        [Column("specialization")]
        public string? Specialization { get; set; }

        /*[ForeignKey(nameof(Department))]
        [Column("dept_id")]
        public int? DeptId { get; set; }

        public Department Dept { get; set; } = null!;*/
    }

    public record DoctorDtoNew
    {
        public required string Name { get; set; }
        public required string MaxQualification { get; set; }
        public required string Specialization { get; set; }
        //public required int DeptId { get; set; }
    }
    public record DoctorDtoPatch
    {
        public string? Name { get; set; }
        public string? MaxQualification { get; set; }
        public string? Specialization { get; set; }
        //public int? DeptId { get; set; }
    }
}
