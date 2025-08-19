namespace hms.Models.DTOs
{
    public class PasswordResetDto
    {
        public required string Password { get; set; }
        public required string Otp { get; set; }
    }
}
