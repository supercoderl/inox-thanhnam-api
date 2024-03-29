using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Email;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net;

namespace InoxThanhNamServer.Services.EmailSer
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };
            return emailMessage;
        }
        private async Task SendEmailAsync(MimeMessage mailMessage)
        {
            using(var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
                    await client.SendAsync(mailMessage);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        public async Task<ApiResponse<string>> SendEmail(Message message)
        {
            try
            {
                var mailMessage = CreateEmailMessage(message);
                await SendEmailAsync(mailMessage);
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Gửi mail thành công",
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "EmailService - SendMail: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
