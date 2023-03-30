using ConsumerService.IntegrationEvents.Events;

namespace ConsumerService.Abstractions;

public interface IEventBus
{
    #region Public Methods

    void Subscribe<T, TH>() 
        where T : IntegrationEvent 
        where TH : IIntegrationEventHandler<T>;

    void UnSubcribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    #endregion
}
