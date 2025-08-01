namespace EmailOTP.Models
{
    public static class EmailStatusCodes
    {
        public const int STATUS_EMAIL_OK = 1000;
        public const int STATUS_EMAIL_FAIL = 1001;
        public const int STATUS_EMAIL_INVALID = 1002;
    }

    public static class OtpStatusCodes
    {
        public const int STATUS_OTP_OK = 2000;
        public const int STATUS_OTP_FAIL = 2001;
        public const int STATUS_OTP_TIMEOUT = 2002;
        public const int STATUS_OTP_INVALID = 2003;

    }
}
