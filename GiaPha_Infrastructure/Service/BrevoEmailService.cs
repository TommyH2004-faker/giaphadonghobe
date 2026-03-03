using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Threading.Tasks;
using TodoApp.Application.Service;

namespace GiaPha_Infrastructure.Service;

public class BrevoEmailService : IEmailService
{
    private readonly ILogger<BrevoEmailService> _logger;
    private readonly TransactionalEmailsApi _apiInstance;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public BrevoEmailService(
        IConfiguration configuration,
        ILogger<BrevoEmailService> logger)
    {
        _logger = logger;

        var apiKey = configuration["Brevo:ApiKey"]
            ?? throw new InvalidOperationException("Brevo ApiKey not configured");
        _fromEmail = configuration["Brevo:FromEmail"]
            ?? throw new InvalidOperationException("Brevo FromEmail not configured");
        _fromName = configuration["Brevo:FromName"] ?? "GiaPha Notification";

        sib_api_v3_sdk.Client.Configuration.Default.ApiKey.Add("api-key", apiKey);
        _apiInstance = new TransactionalEmailsApi();
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var sender = new SendSmtpEmailSender(_fromEmail, _fromName);
            var receiver = new SendSmtpEmailTo(to);

            var sendSmtpEmail = new SendSmtpEmail(
                sender: sender,
                to: new List<SendSmtpEmailTo> { receiver },
                htmlContent: isHtml ? body : null,
                textContent: isHtml ? null : body,
                subject: subject
            );

            var result = await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            _logger.LogInformation("Brevo đã gửi email tới {To}. MessageId: {MessageId}", 
                to, result.MessageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email to {To}", to);
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body, bool isHtml = true)
    {
        try
        {
            var recipients = toList.Where(email => !string.IsNullOrWhiteSpace(email)).ToList();
            if (recipients.Count == 0)
            {
                return false;
            }

            var sender = new SendSmtpEmailSender(_fromEmail, _fromName);
            var receivers = recipients.Select(email => new SendSmtpEmailTo(email)).ToList();

            var sendSmtpEmail = new SendSmtpEmail(
                sender: sender,
                to: receivers,
                htmlContent: isHtml ? body : null,
                textContent: isHtml ? null : body,
                subject: subject
            );

            var result = await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            _logger.LogInformation("Brevo đã gửi email tới {Count} người. MessageId: {MessageId}", 
                recipients.Count, result.MessageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email to multiple recipients");
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
    {
        string subject = "Chào mừng đến với GiaPha DongHo";
        string body = $@"
            <h2>Xin chào {userName},</h2>
            <p>Chào mừng bạn đến với hệ thống GiaPha DongHo!</p>
            <p>Bạn đã đăng ký thành công tài khoản.</p>
            <p>Trân trọng,<br/>Đội ngũ GiaPha DongHo</p>
        ";
        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        string subject = "Đặt lại mật khẩu";
        string body = $@"
            <h2>Yêu cầu đặt lại mật khẩu</h2>
            <p>Bạn đã yêu cầu đặt lại mật khẩu.</p>
            <p>Vui lòng click vào link sau để đặt lại mật khẩu:</p>
            <p><a href='{resetLink}'>Đặt lại mật khẩu</a></p>
            <p>Link này sẽ hết hạn sau 24 giờ.</p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
        ";
        return await SendEmailAsync(toEmail, subject, body);
    }
}
