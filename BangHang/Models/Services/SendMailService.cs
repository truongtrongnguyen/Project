using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BangHang.Models.Services
{

    public class MailSetting
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PassWord { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }


    public class SendMailService : IEmailSender
    {
        private readonly MailSetting _mailSetting;
        private readonly ILogger<SendMailService> _logger;

        public SendMailService(IOptions<MailSetting> mailSetting, ILogger<SendMailService> logger)
        {
            _mailSetting = mailSetting.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Email);
            message.From.Add( new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Email));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            // dùng smtp của MaiKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSetting.Email, _mailSetting.PassWord);
                await smtp.SendAsync(message);
            }
            catch(Exception ex)
            {
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);

                _logger.LogInformation("Lõi gửi Mail, Lưu tại " + emailsavefile);
                _logger.LogInformation(ex.Message);
            }
            smtp.Disconnect(true);
            _logger.LogInformation("Send Mail to " + email);
        }
    }
}
