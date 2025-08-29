using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hms.Models
{
    [Table("chat_sessions")]
    public class ChatSession
    {
        [Column("id")]
        [Required]
        public required Guid Id { get; set; }

        [Column("start_time")]
        [Required]
        public required DateTime StartTime { get; set; }

        [Column("last_activity")]
        [Required]
        public required DateTime LastActivity { get; set; }
    }
}
