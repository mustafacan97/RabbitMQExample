using EventBus.Producer.Abstractions;
using ProducerService.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;
using ProducerService.IntegrationEvents.Events;

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

    [HttpGet(Name="PublishOrderCreatedSuccessIntegrationEvent")]
    public void Publish1()
    {
        _eventBus.Publish(new OrderCreatedSuccessIntegrationEvent());
    }

    [HttpGet(Name = "PublishOrderCreatedFailedIntegrationEvent")]
    public void Publish2()
    {
        _eventBus.Publish(new OrderCreatedFailedIntegrationEvent());
    }
}
