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

        [Column("max_qual")]
        public string? MaxQualification { get; set; }

        [Column("specialization")]
        public string? Specialization { get; set; }
    }

    public class Doctor_New
    {
        public required string Name { get; set; }
        public required string MaxQualification { get; set; }
        public required string Specialization { get; set; }
    }
    public class Doctor_Optional
    {
        public string? Name { get; set; }
        public string? MaxQualification { get; set; }
        public string? Specialization { get; set; }
    }
}
