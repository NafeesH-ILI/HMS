using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("users")]
    public class User
    {
        public enum Types : int
        {
            SuperAdmin,
            Admin,
            Receptionist,
            Doctor,
            Patient
        }

        [Column("uname")]
        public required string UName { get; set; }

        [Column("type")]
        public required Types Type { get; set; }

        [Column("pass")]
        [JsonIgnore]
        public required string PassHash { get; set; }
    }
}
