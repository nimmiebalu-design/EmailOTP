namespace EmailOTP.Services.Interfaces
{
    public interface IEmailService
    {   
        Task<int> generate_OTP_email(string email_address);
        int Check_OTP(string email_address, string otp);
       
    }
}
