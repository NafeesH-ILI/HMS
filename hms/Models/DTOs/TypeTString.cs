using System.ComponentModel.DataAnnotations;

namespace hms.Models.DTOs
{
    public class TypeTString<T>
    {
        public required string Type { get; set; }
    }
}
