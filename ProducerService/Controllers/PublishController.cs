using EventBus.Producer.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ProducerService.Events.IntegrationEvents.Order;

namespace EventBus.Producer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PublishController : ControllerBase
{
    #region Fields

    private readonly IEventBus _eventBus;

    #endregion

    #region Constructors and Destructors

    public PublishController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    #endregion

    #region Public Methods

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

    #endregion
}