using ConsumerService.Abstractions;
using ConsumerService.Concretes;
using ConsumerService.IntegrationEvents.EventHandlers;
using ConsumerService.IntegrationEvents.Events;

namespace ConsumerService.Infrastructure;

public static class RabbitMQConfigurtion
{
    #region Public Methods

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.SectionName));
        services.AddTransient<OrderCreatedSuccessIntegrationEventHandler>();
        services.AddTransient<OrderCreatedFailedIntegrationEventHandler>();
        services.AddScoped<IEventBus, RabbitMQEventBus>();
        SubscribeEvents(services);
        return services;
    }

    #endregion

    #region Methods

    private static void SubscribeEvents(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        IEventBus eventBus = sp.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedSuccessIntegrationEvent, OrderCreatedSuccessIntegrationEventHandler>();
        eventBus.Subscribe<OrderCreatedFailedIntegrationEvent, OrderCreatedFailedIntegrationEventHandler>();
    }

    #endregion
}
