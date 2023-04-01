using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using EventBus.Producer.Abstractions;
using RabbitMQ.Client;
using ProducerService.Events.IntegrationEvents;

namespace EventBus.Producer.Concretes;

public class RabbitMQEventBus : IEventBus
{
    #region Fields
    
    private readonly RabbitMQPersistentConnection _persistentConnection;

    private readonly RabbitMQSettings _rabbitMQSettings;

    private readonly IModel _consumerChannel;

    #endregion

    #region Constructors and Destructors

    public RabbitMQEventBus(IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _rabbitMQSettings = rabbitMQSettings.Value;
        _persistentConnection = new RabbitMQPersistentConnection(rabbitMQSettings);
        _consumerChannel = CreateConsumerChannel();
    }

    #endregion

    #region Public Methods

    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_rabbitMQSettings.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                // TODO: log
            });

        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);

        // ensure exchange exists while publishing
        _consumerChannel.ExchangeDeclare(exchange: _rabbitMQSettings.DefaultTopicName,
                                         type: "direct");

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = _consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent

            _consumerChannel.BasicPublish(exchange: _rabbitMQSettings.DefaultTopicName,
                                          routingKey: eventName,
                                          mandatory: true,
                                          basicProperties: properties,
                                          body: body);
        });
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

    public virtual string ProcessEventName(string eventName)
    {
        if (_rabbitMQSettings.DeleteEventPrefix)
            eventName = eventName.TrimStart(_rabbitMQSettings.EventNamePrefix.ToArray());

        if (_rabbitMQSettings.DeleteEventSuffix)
            eventName = eventName.TrimEnd(_rabbitMQSettings.EventNameSuffix.ToArray());

        return eventName;
    }

    #endregion
}