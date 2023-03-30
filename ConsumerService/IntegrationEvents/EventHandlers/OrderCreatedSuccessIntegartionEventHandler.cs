using ConsumerService.Abstractions;
using ConsumerService.IntegrationEvents.Events;

namespace ConsumerService.IntegrationEvents.EventHandlers;

public class OrderCreatedSuccessIntegartionEventHandler : IIntegrationEventHandler<OrderCreatedSuccessIntegrationEvent>
{
    private readonly ILogger<OrderCreatedSuccessIntegartionEventHandler> _logger;

    public OrderCreatedSuccessIntegartionEventHandler(ILogger<OrderCreatedSuccessIntegartionEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedSuccessIntegrationEvent @event)
    {
        _logger.LogInformation($"Created order id is = {@event.OrderId}");

        return Task.CompletedTask;
    }
}
