using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("pass_reset_otp")]
    public class PassResetOtp
    {
        [Column("id")]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required Guid Id { get; set; }

        [Column("unamme")]
        [Required]
        public required string UName { get; set; }

        [Column("otp")]
        [Required]
        public required string Otp { get; set; }

        [Column("expiry")]
        [Required]
        public DateTime Expiry { get; set; } = DateTime.Now;

        [Column("is_valid")]
        [Required]
        public required bool IsValid { get; set; } = true;
    }
}
