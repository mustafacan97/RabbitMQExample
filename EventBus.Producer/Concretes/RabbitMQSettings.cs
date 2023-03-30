namespace EventBus.Producer.Concretes;

public class RabbitMQSettings
{
    #region Public Properties

    public const string SectionName = "RabbitMQSettings";

    public string HostName { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string DefaultTopicName { get; init; } = string.Empty;

    public int ConnectionRetryCount { get; init; }

    public bool DeleteEventPrefix { get; init; }

    public bool DeleteEventSuffix { get; init; }

    public string EventNamePrefix { get; init; } = string.Empty;

    public string EventNameSuffix { get; init; } = string.Empty;

    #endregion
}
