using System.ComponentModel.DataAnnotations;

namespace hms.Models.DTOs
{
    public class UserDtoGet
    {
        public required string UName { get; set; }

        [EnumDataType(typeof(User.Types))]
        public required string Type { get; set; }
    }
}
