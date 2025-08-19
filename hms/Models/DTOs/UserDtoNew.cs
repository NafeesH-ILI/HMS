using System.ComponentModel.DataAnnotations;

namespace hms.Models.DTOs
{
    public class UserDtoNew
    {
        public required string Name { get; set; }

        [EnumDataType(typeof(User.Types))]
        public required string Type { get; set; }

        public required string Password { get; set; }
    }
}
