using MassTransit.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.DependencyInjection;
using BuildingBlocks.Events;
using Workers.Features.Workers.Consumers;
using Workers.Infrastructure.Persistence;

namespace Workers;

public static class  WorkersModule 
{
    public static WebApplicationBuilder Install(WebApplicationBuilder builder)
    {
        ConfigureTodosDatabase(builder);
        
        builder.AddAssemblyServices(typeof(WorkersModule).Assembly);
        
        builder.Services.RegisterConsumer<GetWorkerConsumer>();

        builder.Services.RegisterConsumer<CreateTodoConsumer>();
        
        return builder;
    }

    private static void ConfigureTodosDatabase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddDbContext<WorkersDbContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging();

            var publishDomainEventsInterceptor =
                serviceProvider.GetRequiredService<PublishDomainEventsInterceptor>();

            optionsBuilder.AddInterceptors(publishDomainEventsInterceptor);
        });
    }
}