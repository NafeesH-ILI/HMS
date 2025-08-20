namespace hms.Models.DTOs
{
    public class PasswordChangeDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
