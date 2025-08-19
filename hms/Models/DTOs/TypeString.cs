using System.ComponentModel.DataAnnotations;

namespace hms.Models.DTOs
{
    public class TypeString
    {
        [EnumDataType(typeof(User.Types), ErrorMessage = "Not a valid User Type")]
        public required string Type { get; set; }
    }
}
