using Microsoft.EntityFrameworkCore;
using ProducerService.Data;

namespace ProducerService.Infrastructure;

public static class EntityFrameworkConfiguration
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        var abc = configuration.GetConnectionString("ProducerServiceDbConnectionString");
        services.AddDbContext<ProducerServiceDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("ProducerServiceDbConnectionString")));
        ApplyMigrations(services);
        return services;
    }

    private static void ApplyMigrations(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProducerServiceDbContext>();
        dbContext.Database.Migrate();
    }
}
