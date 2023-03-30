namespace EventBus.Producer.Events;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId;

	public OrderCreatedIntegrationEvent()
	{
		OrderId = Guid.NewGuid();
	}
}
