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

        var apiKey = configuration["Brevo:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("[Brevo] ApiKey is not configured. Set 'Brevo__ApiKey' environment variable on Render.");

        _fromEmail = configuration["Brevo:FromEmail"];
        if (string.IsNullOrWhiteSpace(_fromEmail))
            throw new InvalidOperationException("[Brevo] FromEmail is not configured. Set 'Brevo__FromEmail' environment variable on Render.");

        _fromName = configuration["Brevo:FromName"] ?? "GiaPha Notification";

        // Tạo instance Configuration riêng, inject trực tiếp vào API client
        // Tránh dùng static Configuration.Default (bị share/overwrite giữa các service)
        var brevoConfig = new sib_api_v3_sdk.Client.Configuration();
        brevoConfig.ApiKey["api-key"] = apiKey;

        _apiInstance = new TransactionalEmailsApi(brevoConfig);
        _logger.LogInformation("[BrevoEmailSender] Initialized. FromEmail='{FromEmail}', FromName='{FromName}'", _fromEmail, _fromName);
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
