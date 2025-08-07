using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hms.Models
{
    [Table("doctors")]
    public class Doctor
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }
    }

    public class Doctor_NoId
    {
        public string? Name { get; set; }
    }

    public class Doctor_NoId_Optional
    {
        public string? Name { get; set; }
    }
}
