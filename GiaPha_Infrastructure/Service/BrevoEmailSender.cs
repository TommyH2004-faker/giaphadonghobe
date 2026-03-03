using GiaPha_Application.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Threading.Tasks;

namespace GiaPha_Infrastructure.Service;

public class BrevoEmailSender : IEmailSender
{
    private readonly ILogger<BrevoEmailSender> _logger;
    private readonly TransactionalEmailsApi _apiInstance;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public BrevoEmailSender(IConfiguration configuration, ILogger<BrevoEmailSender> logger)
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

    public async System.Threading.Tasks.Task SendEmail(string to, string subject, string body)
    {
        try
        {
            var sender = new SendSmtpEmailSender(_fromEmail, _fromName);
            var receiver = new SendSmtpEmailTo(to);
            
            var sendSmtpEmail = new SendSmtpEmail(
                sender: sender,
                to: new List<SendSmtpEmailTo> { receiver },
                htmlContent: body,
                subject: subject
            );

            var result = await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            
            _logger.LogInformation("[BrevoEmailSender] Đã gửi email tới {To}. MessageId: {MessageId}", 
                to, result.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BrevoEmailSender] Lỗi gửi email tới {To}: {Message}", to, ex.Message);
        }
    }
}
