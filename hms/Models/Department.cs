using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("departments")]
    public class Department
    {
        [Column("uname")]
        [Key]
        [Required]
        public required string UName { get; set; }

        [Column("name")]
        [MaxLength(100)]
        [MinLength(2)]
        [Required]
        public string? Name { get; set; }

        [InverseProperty("dept")]
        [JsonIgnore]
        public ICollection<Doctor> Doctors { get; set; } = [];
    }

    public record DepartmentDtoNew
    {
        public required string UName { get; set; }
        public required string Name { get; set; }
    }
    public record DepartmentDtoPut
    {
        public required string Name { get; set; }
    }
}
