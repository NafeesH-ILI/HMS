using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hms.Models
{
    [Table("chat_sessions_users")]
    [PrimaryKey(nameof(ChatSessionId), nameof(UserId))]
    public class ChatSessionUser
    {
        [Column("chat_session_id")]
        [Required]
        public required Guid ChatSessionId { get; set; }

        [Column("user_id")]
        [Required]
        public required string UserId { get; set; }

        [ForeignKey(nameof(ChatSessionId))]
        public ChatSession? ChatSession { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
