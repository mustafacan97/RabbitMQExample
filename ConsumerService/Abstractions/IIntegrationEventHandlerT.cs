using ConsumerService.IntegrationEvents.Events;

namespace ConsumerService.Abstractions;

public interface IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}