using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("users")]
    public class User
    {
        enum Types 
        {
            SuperAdmin,
            Admin,
            Receptionist,
            Doctor,
            Patient
        }

        [Column("uname")]
        public required string UName { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("pass_hash")]
        [JsonIgnore]
        public required string PassHash { get; set; }
    }
}
