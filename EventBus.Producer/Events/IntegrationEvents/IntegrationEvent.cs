using System.Text.Json.Serialization;

namespace ProducerService.Events.IntegrationEvents;

public class IntegrationEvent
{
    [JsonInclude]
    public Guid Id { get; private set; }

    [JsonInclude]
    public DateTime CreatedDate { get; private set; }

    [JsonConstructor]
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public IntegrationEvent(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }
}