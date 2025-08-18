using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace hms.Models
{
    [Table("unames")]
    public class UName
    {
        [Column("name")]
        [Required]
        [Key]
        public required string Name { get; set; }

        [Column("count")]
        [Required]
        public required int Count { get; set; }
    }
}
