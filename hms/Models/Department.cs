using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("departments")]
    public class Department
    {
        [Column("uname")]
        [Key]
        public required string UName { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        //[InverseProperty(nameof(Doctor))]
        //public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
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
