using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("patients")]
    public class Patient
    {
        [Required]
        [Key]
        public required string UName { get; set; }

        [Required]
        [Column("phone")]
        [Length(12, 12)]
        public required string Phone { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [Column("dob")]
        public DateOnly? DateBirth { get; set; }
    }
}
