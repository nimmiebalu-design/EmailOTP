using EmailOTP.Models;
using EmailOTP.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Runtime;

namespace EmailOTP.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
   
    public class EmailService:IEmailService
    {

        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        private static Dictionary<string, OtpModel> otpStore = new();

        public async Task<int> generate_OTP_email(string email_address)
        {
            if (string.IsNullOrWhiteSpace(email_address) || !email_address.Contains("@") || !email_address.Contains("@dso.org.sg"))
            {
                return EmailStatusCodes.STATUS_EMAIL_INVALID;

            }
            var otp = generate_OTP(email_address);
            string body = $"Your OTP code is { otp }. It will expire in 1 minute";
            return await send_email(email_address, body);
        }

        public string generate_OTP(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            otpStore[email] = new OtpModel
            {
                Email = email,
                Otp = otp,
                GeneratedAt = DateTime.UtcNow,
                AttemptCount = 0
            };

            return otp;
        }

        // Validate OTP
        public int Check_OTP(string email, string inputOtp)
        {
            if (!otpStore.ContainsKey(email))
                return EmailStatusCodes.STATUS_EMAIL_INVALID;

            var model = otpStore[email];

            // Check expiration
            if ((DateTime.UtcNow - model.GeneratedAt).TotalMinutes > 1)
            {
                otpStore.Remove(email);
                return OtpStatusCodes.STATUS_OTP_TIMEOUT;
            }

            // Check attempt limit
            if (model.AttemptCount >= 10)
            {
                otpStore.Remove(email);
                return OtpStatusCodes.STATUS_OTP_FAIL;
            }

            // Check OTP match
            if (model.Otp != inputOtp)
            {
                model.AttemptCount++;
                return OtpStatusCodes.STATUS_OTP_INVALID;
            }

            // Success
            otpStore.Remove(email);
            return OtpStatusCodes.STATUS_OTP_OK;
        }
        private async Task<int> send_email(string email_address, string body)
        {
            try
            {
                using (var client = new SmtpClient(_settings.SmtpServer, _settings.Port))
                {
                    client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                    client.EnableSsl = true;
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                        Subject = "Your OTP Code",
                        Body = body,
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(email_address);
                    client.Send(_settings.SenderEmail, email_address, mailMessage.Subject, mailMessage.Body);
                    return EmailStatusCodes.STATUS_EMAIL_OK;
                }
            }
            catch (Exception ex)
            {
                return EmailStatusCodes.STATUS_EMAIL_FAIL;
            }
        }

    }
    }
