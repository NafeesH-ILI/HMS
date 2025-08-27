using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace hms.Models
{
    [Table("users")]
    public class User : IdentityUser
    {
        public enum Types : int
        {
            SuperAdmin,
            Admin,
            Receptionist,
            Doctor,
            Patient
        }
        public required Types Type { get; set; }

        [Column("is_active")]
        [DefaultValue(true)]
        public required bool IsActive { get; set; } = true;
    }
}
