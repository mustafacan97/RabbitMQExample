using ConsumerService.Abstractions;
using ConsumerService.IntegrationEvents.Events;
using ConsumerService.Models;

namespace EventBus.Base.SubManagers;

public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
{
    #region Fields    

    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

    private readonly List<Type> _eventTypes;

    public event EventHandler<string>? OnEventRemoved;

    public Func<string, string> _eventNameGetter;

    #endregion

    #region Constructors and Destructors

    public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
        _eventNameGetter = eventNameGetter;
    }

    #endregion

    #region Public Methods

    public bool IsEmpty => _handlers.Keys.Any();

    public void Clear() => _handlers.Clear();

    public void AddSubscription<T, TH>()
    where T : IntegrationEvent
    where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();

        AddSubscription(typeof(TH), eventName);

        if(!_eventTypes.Contains(typeof(TH)))
            _eventTypes.Add(typeof(TH));
    }

    public string GetEventKey<T>() where T : IntegrationEvent
    {
        string eventName = typeof(T).Name;
        return _eventNameGetter(eventName);
    }

    public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var handlerToRemove = FindSubscriptionToRemove<T, TH>();
        if (handlerToRemove is null) return;
        var eventName = GetEventKey<T>();
        RemoveHandler(eventName, handlerToRemove);
    }

    #endregion

    #region Methods

    private void AddSubscription(Type handerType, string eventName)
    {
        if (!HasSubscriptionsForEvent(eventName))
            _handlers.Add(eventName, new List<SubscriptionInfo>());

        if (_handlers[eventName].Any(s => s.HandlerType == handerType))
            throw new ArgumentException($"Handler type {handerType.Name} already registered for '{eventName}'", nameof(handerType));

        _handlers[eventName].Add(SubscriptionInfo.Create(handerType));
    }

    private SubscriptionInfo? FindSubscriptionToRemove<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        return FindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo? FindSubscriptionToRemove(string eventName, Type handlerType)
    {
        if (!HasSubscriptionsForEvent(eventName))
            return null;
        return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
    }

    private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
    {
        _handlers[eventName].Remove(subsToRemove);

        if (!_handlers[eventName].Any())
        {
            _handlers.Remove(eventName);
            var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
            if (eventType is not null)
                _eventTypes.Remove(eventType);

            RaiseOnEventRemoved(eventName);
        }
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }

    #endregion
}
