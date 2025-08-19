namespace hms.Models.DTOs
{
    public class PasswordResetDto
    {
        public required string Password { get; set; }
        public required string Otp { get; set; }
        public required Guid SessionId { get; set; }
    }
}
