using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Events;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services
            .AddScoped<PublishDomainEventsInterceptor>();

        return services;
    }
}