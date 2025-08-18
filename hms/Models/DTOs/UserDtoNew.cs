namespace hms.Models.DTOs
{
    public class UserDtoNew
    {
        public required string UName { get; set; }
        public required User.Types Type { get; set; }
        public required string Password { get; set; }
    }
}
