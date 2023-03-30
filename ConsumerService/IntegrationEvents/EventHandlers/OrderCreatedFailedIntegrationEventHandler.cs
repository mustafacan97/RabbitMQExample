using ConsumerService.Abstractions;
using ConsumerService.IntegrationEvents.Events;

namespace ConsumerService.IntegrationEvents.EventHandlers;

public class OrderCreatedFailedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedFailedIntegrationEvent>
{
    private readonly ILogger<OrderCreatedFailedIntegrationEventHandler> _logger;

    public OrderCreatedFailedIntegrationEventHandler(ILogger<OrderCreatedFailedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedFailedIntegrationEvent @event)
    {
        _logger.LogError("Order created is failed.");

        return Task.CompletedTask;
    }
}
