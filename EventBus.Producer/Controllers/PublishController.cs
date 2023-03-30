using EventBus.Producer.Abstractions;
using EventBus.Producer.Events;
using Microsoft.AspNetCore.Mvc;

namespace EventBus.Producer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PublishController : ControllerBase
{
    private readonly IEventBus _eventBus;

    public PublishController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [HttpGet(Name="PublishOrderCreatedIntegrationEvent")]
    public void Publish1()
    {
        _eventBus.Publish(new OrderCreatedIntegrationEvent());
    }
}
