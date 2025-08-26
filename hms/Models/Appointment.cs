using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("appointments")]
    public class Appointment
    {
        public enum Statuses
        {
            Scheduled,
            Cancelled,
            Done
        }

        [Column("id")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required Guid Id { get; set; }

        [Column("status")]
        [Required]
        public required Statuses Status { get; set; } = Statuses.Scheduled;

        [Column("time", TypeName = "timestamp with time zone")]
        [Required]
        public required DateTime Time { get; set; }

        [Column("patient_id")]
        [Required]
        public required string PatientId { get; set; }

        [Column("doctor_id")]
        [Required]
        public required string DoctorId { get; set; }

        [ForeignKey("PatientId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Patient? Patient { get; set; }

        [ForeignKey("PatientId")]
        [JsonIgnore]
        public User? PatientUser { get; set; }

        [ForeignKey("DoctorId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Doctor? Doctor { get; set; }

        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public User? DoctorUser { get; set; }
    }
}
