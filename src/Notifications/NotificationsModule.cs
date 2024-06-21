using Microsoft.AspNetCore.Builder;
using BuildingBlocks.DependencyInjection;
using MassTransit.Configuration;
using Microsoft.AspNetCore.SignalR;
using Notifications.Infrastructure.Hubs;

namespace Notifications;

public static class NotificationsModule
{
    public static WebApplicationBuilder Install(WebApplicationBuilder builder)
    {
        builder.AddAssemblyServices(typeof(NotificationsModule).Assembly);

        builder.Services.RegisterConsumer<TodoCompletedEventConsumer>();
        builder.Services.RegisterConsumer<TodoWorkerAssigmentConsumer>();

        return builder;
    }

    public static WebApplication Map(WebApplication app)
    {
        app.MapHub<TodoHub>("hubs/todo");

        return app;
    }
}