using EventBus.Producer.Abstractions;
using EventBus.Producer.Concretes;

namespace ProducerService.Infrastructure;

public static class RabbitMQConfiguration
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.SectionName));
        services.AddScoped<IEventBus, RabbitMQEventBus>();
        return services;
    }
}
