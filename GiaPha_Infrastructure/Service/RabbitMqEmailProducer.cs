using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using GiaPha_Application.IntegrationEvents;
using GiaPha_Application.Service;
using Microsoft.Extensions.Logging;

public class RabbitMqEmailProducer : IRabbitMqEmailProducer
{
    private readonly string _uri;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMqEmailProducer>? _logger;

    public RabbitMqEmailProducer(string uri, ILogger<RabbitMqEmailProducer>? logger = null)
    {
        _uri = uri;
        _logger = logger;
    }

    private void EnsureConnection()
    {
        if (_connection != null && _connection.IsOpen)
            return;

        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_uri),
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

            _logger?.LogInformation("RabbitMQ connection established");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to connect to RabbitMQ. Email notifications will be skipped.");
        }
    }

    public void Publish(SendEmailIntegrationEvent emailEvent)
    {
        try
        {
            EnsureConnection();

            if (_channel == null || !_channel.IsOpen)
            {
                _logger?.LogWarning("RabbitMQ channel not available. Skipping email notification.");
                return;
            }

            var message = JsonSerializer.Serialize(emailEvent);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // 🔥 Quan trọng: đảm bảo message không mất

            _channel.BasicPublish(
                exchange: "",
                routingKey: "send-email-queue",
                basicProperties: properties,
                body: body);

            _logger?.LogInformation("Email notification published to queue");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to publish email notification to RabbitMQ");
        }
    }
}