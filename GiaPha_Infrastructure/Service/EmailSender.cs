using System.Net.Mail;
using GiaPha_Application.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GiaPha_Infrastructure.Service;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        try
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var fromEmail = smtpSection["FromEmail"] ;
            var fromName = smtpSection["FromName"];
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"] ?? "587");
            var username = smtpSection["Username"];
            var password = smtpSection["Password"];

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new ArgumentNullException(nameof(fromEmail), "FromEmail configuration value cannot be null or empty.");
            }

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            using var smtp = new SmtpClient(host)
            {
                Port = port,
                Credentials = new System.Net.NetworkCredential(username, password),
                EnableSsl = true
            };
            await smtp.SendMailAsync(mail);
            _logger.LogInformation(" [EmailSender] Đã gửi email tới {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " [EmailSender] Lỗi gửi email tới {To}: {Message}", to, ex.Message);
        }
    }
}