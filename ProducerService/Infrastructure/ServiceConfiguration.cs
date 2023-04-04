using ProducerService.Primitives;

namespace ProducerService.Infrastructure;

public static class ServiceConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        return services;
    }
}
