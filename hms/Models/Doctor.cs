using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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
        public required string Name { get; set; }

        [Column("max_qual")]
        [Required]
        [MaxLength(50)]
        public required string MaxQualification { get; set; }

        [Column("specialization")]
        [Required]
        [MaxLength(50)]
        public required string Specialization { get; set; }

        [Column("dept")]
        [Required]
        public required string DeptKey { get; set; }

        [ForeignKey("DeptKey")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Department Dept { get; set; } = null!;

        [ForeignKey("UName")]
        public User User { get; set; } = null!;
    }
  
}
