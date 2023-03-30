namespace ProducerService.IntegrationEvents.Events;

public class OrderCreatedFailedIntegrationEvent : IntegrationEvent
{
    public string Status = "Failed";
}
