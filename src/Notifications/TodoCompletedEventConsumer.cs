using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Notifications.Infrastructure.Hubs;

namespace Notifications;

public class TodoCompletedEventConsumer(ILogger<TodoCompletedEventConsumer> logger,IRequestClient<GetWorkerByNameRequest> client, IHubContext<TodoHub, ITodoHub> hubContext) : IConsumer<TodoCompletedEvent>
{
    public async Task Consume(ConsumeContext<TodoCompletedEvent> context)
    {
        Thread.Sleep(TimeSpan.FromSeconds(3));
        
        logger.LogInformation("(NOTIFICATIONS - INTEGRATION) --- Todo {TodoId} has been completed, ", context.Message.Id);
        
        var user = await client.GetResponse<GetWorkerByNameResponse>( new GetWorkerByNameRequest { WorkerId = context.Message.Worker });

        await hubContext.Clients.All.SendCompleted(new TodoCompleted
        {
            Title = context.Message.Title,
            User = user.Message.Name
        });
        
        logger.LogInformation("(NOTIFICATIONS - INTEGRATION) --- Notification sent to all clients for Todo {TodoId}", context.Message.Id);
        
    }
}