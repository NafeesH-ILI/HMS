using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
using System.Text.Json.Serialization;
=======
>>>>>>> master

namespace hms.Models.DTOs
{
    public class UserDtoGet
    {
        public required string UName { get; set; }

        [EnumDataType(typeof(User.Types))]
        public required string Type { get; set; }
<<<<<<< HEAD

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Doctor? Doctor { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Patient? Patient { get; set; }
=======
>>>>>>> master
    }
}
