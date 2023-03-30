using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace ConsumerService.Concretes;

public class RabbitMQPersistentConnection : IDisposable
{
    #region Fields

    private readonly RabbitMQSettings _rabbitMQSettings;

    private readonly IConnectionFactory _connectionFactory;

    private IConnection _connection;

    private bool _disposed;

    private readonly object lock_object = new();

    #endregion

    #region Constructors and Destructors

    public RabbitMQPersistentConnection(IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _rabbitMQSettings = rabbitMQSettings.Value;
        _connectionFactory = new ConnectionFactory()
        {
            HostName = _rabbitMQSettings.HostName,
            UserName = _rabbitMQSettings.UserName,
            Password = _rabbitMQSettings.Password
        };
    }

    #endregion

    #region PUblic Methods

    public bool IsConnected => _connection is not null && _connection.IsOpen;

    public IModel CreateModel() => _connection.CreateModel();

    public void Dispose()
    {
        _connection?.Dispose();
        _disposed = true;
    }

    public bool TryConnect()
    {
        lock (lock_object)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_rabbitMQSettings.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (Exception, time) =>
                {
                }
            );

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });

            if (IsConnected)
            {
                // TODO: add log here, we have connected to RabbitMQ

                _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                _connection.CallbackException += _connection_CallbackException;
                _connection.ConnectionBlocked += _connection_ConnectionBlocked;
                return true;
            }

            return false;
        }
    }

    #endregion

    #region Methods

    private void _connection_ConnectionBlocked(object? sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        TryConnect();
    }

    private void _connection_CallbackException(object? sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        TryConnect();
    }

    private void _connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        // TODO: log connection shutdown

        if (_disposed) return;

        TryConnect();
    }

    #endregion
}
