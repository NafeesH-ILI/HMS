using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("departments")]
    public class Department
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        //[InverseProperty(nameof(Doctor))]
        //public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
    
    public record DepartmentDtoNew
    {
        public required string Name { get; set; }
    }
}
