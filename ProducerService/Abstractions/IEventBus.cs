using ProducerService.Events.IntegrationEvents;

namespace EventBus.Producer.Abstractions;

public interface IEventBus
{
    #region Public Methods

    void Publish(IntegrationEvent @event);

    #endregion
}
