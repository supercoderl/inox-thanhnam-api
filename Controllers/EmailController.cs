using InoxThanhNamServer.Datas.Email;
using InoxThanhNamServer.Services.EmailSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost("send-mail")]
        public async Task<IActionResult> Send(MailRequest newMail)
        {
            var message = new Message(new string[] { newMail.To }, "Phản hồi từ Inox Thành Nam", newMail.Content);
            var result = await _emailService.SendEmail(message);
            return StatusCode(result.Status, result);
        }
    }
}
