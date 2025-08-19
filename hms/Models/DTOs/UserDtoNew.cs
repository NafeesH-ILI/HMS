using System.ComponentModel.DataAnnotations;

namespace hms.Models.DTOs
{
    public class UserDtoNew
    {
        public required string Name { get; set; }

        [EnumDataType(typeof(User.Types))]
        public required string Type { get; set; }
<<<<<<< HEAD

=======
>>>>>>> master
        public required string Password { get; set; }
    }
}
