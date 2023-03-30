using ConsumerService.Abstractions;
using ConsumerService.IntegrationEvents.Events;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ConsumerService.Concretes;

public class RabbitMQEventBus : IEventBus
{
    #region Fields

    private readonly RabbitMQPersistentConnection _persistentConnection;

    private readonly RabbitMQSettings _rabbitMQSettings;

    private readonly IModel _consumerChannel;

    public readonly IEventBusSubscriptionManager _subsManager;

    public readonly IServiceProvider _serviceProvider;

    #endregion

    #region Constructors and Destructors

    public RabbitMQEventBus(IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _rabbitMQSettings = rabbitMQSettings.Value;
        _persistentConnection = new RabbitMQPersistentConnection(rabbitMQSettings);
        _consumerChannel = CreateConsumerChannel();
        _subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        _subsManager.OnEventRemoved += _subsManager_OnEventRemoved;
    }

    #endregion

    #region Public Methods

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!_subsManager.HasSubscriptionsForEvent(eventName))
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            _consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                                          durable: true,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

            _consumerChannel.QueueBind(queue: GetSubName(eventName),
                                       exchange: _rabbitMQSettings.DefaultTopicName,
                                       routingKey: eventName);
        }

        _subsManager.AddSubscription<T, TH>();
        StartBasicConsume(eventName);
    }

    public void UnSubcribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        _subsManager.RemoveSubscription<T, TH>();
    }

    #endregion

    #region Methods

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        var channel = _persistentConnection.CreateModel();
        channel.ExchangeDeclare(exchange: _rabbitMQSettings.DefaultTopicName,
                                type: "direct");

        return channel;
    }

    private string ProcessEventName(string eventName)
    {
        if (_rabbitMQSettings.DeleteEventPrefix)
            eventName = eventName.TrimStart(_rabbitMQSettings.EventNamePrefix.ToArray());

        if (_rabbitMQSettings.DeleteEventSuffix)
            eventName = eventName.TrimEnd(_rabbitMQSettings.EventNameSuffix.ToArray());

        return eventName;
    }

    private string GetSubName(string eventName) => $"{_rabbitMQSettings.ServiceName}.{ProcessEventName(eventName)}";

    private void StartBasicConsume(string eventName)
    {
        if (_consumerChannel is null) return;

        var consumer = new EventingBasicConsumer(_consumerChannel);
        consumer.Received += Consumer_Received;
        _consumerChannel.BasicConsume(queue: GetSubName(eventName),
                                      autoAck: false,
                                      consumer: consumer);
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        eventName = ProcessEventName(eventName);
        var message = Encoding.UTF8.GetString(e.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch
        {
            // TODO: log here
        }

        _consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
    }

    private async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);

        var processed = false;

        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetService(subscription.HandlerType);
                    if (handler is null) continue;

                    var eventType = _subsManager.GetEventTypeByName($"{_rabbitMQSettings.EventNamePrefix}{eventName}{_rabbitMQSettings.EventNameSuffix}");
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType);

                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }

            processed = true;
        }

        return processed;
    }

    private void _subsManager_OnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        _consumerChannel.QueueUnbind(queue: eventName,
                                     exchange: _rabbitMQSettings.DefaultTopicName,
                                     routingKey: eventName);

        if (_subsManager.IsEmpty)
            _consumerChannel.Close();
    }

    #endregion
}
