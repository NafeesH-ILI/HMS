using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("patients")]
    public class Patient
    {
        [Column("id")]
        [Required]
        [Key]
        public required string Id { get; set; }

        [Required]
        [Column("phone")]
        [Length(12, 12)]
        public required string Phone { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [Column("dob")]
        public required DateOnly DateBirth { get; set; }

        [ForeignKey("Id")]
        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}
