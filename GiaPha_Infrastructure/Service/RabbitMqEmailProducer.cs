using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using GiaPha_Application.Common;
using GiaPha_Application.IntegrationEvents;
using GiaPha_Application.Service;
using Microsoft.Extensions.Logging;

public class RabbitMqEmailProducer : IRabbitMqEmailProducer
{
    private readonly string? _uri;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMqEmailProducer>? _logger;
    private readonly IEmailSender _fallbackSender;
    private readonly bool _isConfigured;

    public RabbitMqEmailProducer(string? uri, IEmailSender fallbackSender, ILogger<RabbitMqEmailProducer>? logger = null)
    {
        _uri = uri;
        _fallbackSender = fallbackSender;
        _logger = logger;
        _isConfigured = !string.IsNullOrWhiteSpace(uri);

        if (!_isConfigured)
            _logger?.LogInformation("[EmailProducer] RabbitMQ URI not configured. Emails will be sent directly via Brevo.");
    }

    private void EnsureConnection()
    {
        if (_connection != null && _connection.IsOpen)
            return;

        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_uri!),
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "send-email-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger?.LogInformation("[EmailProducer] RabbitMQ connection established.");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "[EmailProducer] Failed to connect to RabbitMQ.");
        }
    }

    public void Publish(SendEmailIntegrationEvent emailEvent)
    {
        // --- Fallback: gửi trực tiếp qua Brevo khi không có RabbitMQ ---
        if (!_isConfigured)
        {
            _logger?.LogInformation("[EmailProducer] Sending email directly via Brevo to {To}.", emailEvent.To);
            _ = _fallbackSender.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.Body);
            return;
        }

        try
        {
            EnsureConnection();

            if (_channel == null || !_channel.IsOpen)
            {
                _logger?.LogWarning("[EmailProducer] RabbitMQ unavailable. Falling back to Brevo direct send.");
                _ = _fallbackSender.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.Body);
                return;
            }

            var message = JsonSerializer.Serialize(emailEvent);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: "",
                routingKey: "send-email-queue",
                basicProperties: properties,
                body: body);

            _logger?.LogInformation("[EmailProducer] Email notification published to queue.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[EmailProducer] Failed to publish to RabbitMQ. Falling back to Brevo.");
            _ = _fallbackSender.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.Body);
        }
    }
}
