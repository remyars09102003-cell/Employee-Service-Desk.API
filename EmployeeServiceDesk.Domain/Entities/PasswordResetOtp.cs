namespace EmployeeServiceDesk.Domain.Entities
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        // Store the hash of the OTP, not the actual OTP.
        public string OtpHash { get; set; } = string.Empty;

        // OTP expiration time in UTC.
        public DateTime ExpiresAtUtc { get; set; }

        // Limit the number of verification attempts.
        public int FailedAttempts { get; set; } = 0;

        // Mark an OTP as used so it cannot be reused.
        public bool IsUsed { get; set; } = false;

        // Store the creation time in UTC.
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}