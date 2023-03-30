using ConsumerService.Abstractions;
using ConsumerService.IntegrationEvents.Events;

namespace ConsumerService.IntegrationEvents.EventHandlers;

public class OrderCreatedSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedSuccessIntegrationEvent>
{
    private readonly ILogger<OrderCreatedSuccessIntegrationEventHandler> _logger;

    public OrderCreatedSuccessIntegrationEventHandler(ILogger<OrderCreatedSuccessIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedSuccessIntegrationEvent @event)
    {
        _logger.LogInformation($"Created order id is = {@event.OrderId}");

        return Task.CompletedTask;
    }
}
