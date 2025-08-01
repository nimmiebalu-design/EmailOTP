using EmailOTP.Models;
using EmailOTP.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailOTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        public readonly IEmailService _emailService;
        public EmailsController(IEmailService emailService)
        {
            // Constructor logic can be added here if needed
            this._emailService = emailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> generate_OTP_email([FromQuery] string email)
        {
            int result = await _emailService.generate_OTP_email(email);
            return result switch
            {
                EmailStatusCodes.STATUS_EMAIL_OK => Ok(new { status = result, message = "OTP has been sent successfully." }),
                EmailStatusCodes.STATUS_EMAIL_INVALID => BadRequest(new { status = result, message = "email address is invalid." }),
                EmailStatusCodes.STATUS_EMAIL_FAIL => StatusCode(500, new { status = result, message = "email address does not exist or sending to the email has failed" }),
                _ => StatusCode(500, new { status = -1, message = "Unknown error." })
            };
        }
        [HttpPost("verify")]
        public IActionResult Check_OTP([FromQuery] string email, [FromQuery] string otp)
        {
            int result = _emailService.Check_OTP(email, otp);
             return result switch
             {
                 OtpStatusCodes.STATUS_OTP_OK => Ok(new { status = result, message = "OTP is valid and checked." }),
                    OtpStatusCodes.STATUS_OTP_INVALID => BadRequest(new { status = result, message = "OTP is wrong." }),
                 OtpStatusCodes.STATUS_OTP_FAIL => BadRequest(new { status = result, message = "OTP is wrong after 10 tries." }),
                 OtpStatusCodes.STATUS_OTP_TIMEOUT => BadRequest(new { status = result, message = "timeout after 1 min." }),
                 _ => StatusCode(500, new { status = -1, message = "Unknown OTP error." })
             };
        }

    }
}
