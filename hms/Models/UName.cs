using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace hms.Models
{
    [Table("unames")]
    [PrimaryKey(nameof(Name), nameof(Table))]
    public class UName
    {
        [Column("name")]
        [Required]
        public required string Name { get; set; }

        [Column("table")]
        [Required]
        public required string Table { get; set; }

        [Column("count")]
        [Required]
        public required int Count { get; set; }
    }
}
