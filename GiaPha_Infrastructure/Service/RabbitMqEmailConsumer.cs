using System.Text;
using System.Text.Json;
using GiaPha_Application.Common;
using GiaPha_Application.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMqEmailConsumer : BackgroundService
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqEmailConsumer> _logger;

    public RabbitMqEmailConsumer(
        IConfiguration configuration,
        IEmailSender emailSender,
        ILogger<RabbitMqEmailConsumer> logger)
    {
        _configuration = configuration;
        _emailSender = emailSender;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Nếu không có URI cấu hình → không khởi động consumer, email sẽ gửi trực tiếp qua Brevo
        var uri = _configuration["RabbitMQ:Uri"];
        if (string.IsNullOrWhiteSpace(uri))
        {
            _logger.LogInformation("[EmailConsumer] RabbitMQ URI not configured. Consumer disabled - emails sent directly via Brevo.");
            return;
        }

        // Retry connection với backoff
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!TryConnect())
                {
                    _logger.LogWarning("[EmailConsumer] RabbitMQ not available. Retrying in 30 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                _logger.LogInformation("[EmailConsumer] RabbitMQ consumer started successfully");
                
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var emailEvent = JsonSerializer.Deserialize<SendEmailIntegrationEvent>(message);

                        if (emailEvent != null)
                        {
                            await _emailSender.SendEmail(
                                emailEvent.To,
                                emailEvent.Subject,
                                emailEvent.Body);
                        }

                        _channel?.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing email from queue");
                        var retryCount = GetRetryCount(ea);

                        if (retryCount >= 3)
                        {
                            _channel?.BasicReject(ea.DeliveryTag, requeue: false);
                        }
                        else
                        {
                            _channel?.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                };

                _channel?.BasicConsume(
                    queue: "send-email-queue",
                    autoAck: false,
                    consumer: consumer);

                // Keep running until cancelled or connection drops
                while (!stoppingToken.IsCancellationRequested && _connection != null && _connection.IsOpen)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }

                _logger.LogWarning("[EmailConsumer] RabbitMQ connection lost. Reconnecting...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailConsumer] Error in RabbitMQ consumer. Retrying in 30 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    private bool TryConnect()
    {
        try
        {
            var uri = _configuration["RabbitMQ:Uri"];
            var factory = new ConnectionFactory
            {
                Uri = new Uri(uri!),
                DispatchConsumersAsync = false
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: "send-email-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[EmailConsumer] Failed to connect to RabbitMQ");
            return false;
        }
    }

    private int GetRetryCount(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.Headers != null &&
            ea.BasicProperties.Headers.TryGetValue("x-death", out var value))
        {
            var deaths = value as List<object>;
            return deaths?.Count ?? 0;
        }

        return 0;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}