using GiaPha_Application.IntegrationEvents;

namespace GiaPha_Application.Service;
public interface IRabbitMqEmailProducer
{
    void Publish(SendEmailIntegrationEvent emailEvent);
}