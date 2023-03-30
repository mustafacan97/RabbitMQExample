﻿using System.Text.Json.Serialization;

namespace ProducerService.IntegrationEvents.Events;

public class OrderCreatedSuccessIntegrationEvent : IntegrationEvent
{
    [JsonInclude]
    public string Status = "Success";

    [JsonInclude]
    public Guid OrderId = Guid.NewGuid();
}
