using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("doctors")]
    public class Doctor
    {
        [Column("uname")]
        [Key]
        [Required]
        public required string UName { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        public string? Name { get; set; }

        [Column("max_qual")]
        [Required]
        [MaxLength(50)]
        public string? MaxQualification { get; set; }

        [Column("specialization")]
        [Required]
        [MaxLength(50)]
        public string? Specialization { get; set; }

        [Column("dept")]
        [Required]
        public string? DeptKey { get; set; }

        [ForeignKey("DeptKey")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Department Dept { get; set; } = null!;
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
