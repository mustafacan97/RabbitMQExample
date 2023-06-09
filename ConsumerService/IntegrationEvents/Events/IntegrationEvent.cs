﻿namespace ConsumerService.IntegrationEvents.Events;

public class IntegrationEvent
{
    public Guid Id { get; private set; }

    public DateTime CreatedDate { get; private set; }

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