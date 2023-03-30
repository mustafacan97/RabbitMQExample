using EventBus.Producer.Events;

namespace EventBus.Producer.Abstractions;

public interface IEventBus
{
    #region Public Methods

    void Publish(IntegrationEvent @event);

    #endregion
}
