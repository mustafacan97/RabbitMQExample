using ConsumerService.IntegrationEvents.Events;
using ConsumerService.Models;

namespace ConsumerService.Abstractions;

public interface IEventBusSubscriptionManager
{
    #region Public Methods    

    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

    bool HasSubscriptionsForEvent(string eventName);

    Type GetEventTypeByName(string eventName);

    void Clear();

    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

    string GetEventKey<T>() where T : IntegrationEvent;

    #endregion

    #region Public Properties

    bool IsEmpty { get; }

    event EventHandler<string> OnEventRemoved;

    #endregion
}
