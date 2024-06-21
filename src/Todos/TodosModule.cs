using System.Data;
using MassTransit.Configuration;
using Microsoft.Data.SqlClient;
using BuildingBlocks.DependencyInjection;
using BuildingBlocks.Events;
using Todos.Diagnostics;
using Todos.Features.Todo.Consumers;
using Todos.Infrastructure.Persistence;

namespace Todos;

public static class TodosModule 
{
    public static WebApplicationBuilder Install(WebApplicationBuilder builder)
    {
        ConfigureTodosDatabase(builder);
        
        builder.Services.AddSingleton<Diagnostics.Diagnostics>();
        
        builder.Services.AddOpenTelemetry().WithMetrics(x => x.AddMeter(Instrumentation.ServiceName));
        
        builder.AddAssemblyServices(typeof(TodosModule).Assembly);
        
        builder.Services.RegisterConsumer<TodoWorkerAssigmentConsumer>();

        return builder;
    }

    private static void ConfigureTodosDatabase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddDbContext<TodoDbContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging();

            var publishDomainEventsInterceptor =
                serviceProvider.GetRequiredService<PublishDomainEventsInterceptor>();

            optionsBuilder.AddInterceptors(publishDomainEventsInterceptor);
        });
            

        builder.Services.AddDbContext<ReadOnlyTodoDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString));
    }
}