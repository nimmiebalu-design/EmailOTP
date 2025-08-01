namespace EmailOTP.Models
{
    public class OtpModel
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime GeneratedAt { get; set; }
        public int AttemptCount { get; set; } = 0;
    }
}
