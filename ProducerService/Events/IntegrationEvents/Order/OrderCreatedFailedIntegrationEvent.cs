namespace ProducerService.Events.IntegrationEvents.Order;

public class OrderCreatedFailedIntegrationEvent : IntegrationEvent
{
    public string Status = "Failed";
}
