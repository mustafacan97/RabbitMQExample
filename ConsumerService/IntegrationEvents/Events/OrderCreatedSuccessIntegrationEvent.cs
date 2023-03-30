namespace ConsumerService.IntegrationEvents.Events;

public class OrderCreatedSuccessIntegrationEvent : IntegrationEvent
{
    public string Status = "Success";

    public Guid OrderId = Guid.NewGuid();
}
