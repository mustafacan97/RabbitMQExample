using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.Producer.Concretes;

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
        Dispose(true);
        GC.SuppressFinalize(this);
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

                _connection.ConnectionShutdown += Connection_ConnectionShutdown;
                _connection.CallbackException += Connection_CallbackException;
                _connection.ConnectionBlocked += Connection_ConnectionBlocked;
                return true;
            }

            return false;
        }
    }

    #endregion

    #region Methods

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
            _disposed = true;
        }
    }

    private void Connection_ConnectionBlocked(object? sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        TryConnect();
    }

    private void Connection_CallbackException(object? sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        TryConnect();
    }

    private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        // TODO: log connection shutdown

        if (_disposed) return;

        TryConnect();
    }

    #endregion
}
