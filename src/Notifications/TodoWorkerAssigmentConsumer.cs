using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Notifications.Infrastructure.Hubs;

namespace Notifications;

internal class TodoWorkerAssigmentConsumer(ILogger<TodoWorkerAssigmentEvent> logger, IRequestClient<GetWorkerByNameRequest> client, IHubContext<TodoHub, ITodoHub> hubContext) : IConsumer<TodoWorkerAssigmentEvent>
{
    public async Task Consume(ConsumeContext<TodoWorkerAssigmentEvent> context)
    {
        logger.LogInformation("(NOTIFICATIONS - INTEGRATION) --- Worker assigned to todo {TodoId}", context.Message.TodoId);
        
        var worker = await client.GetResponse<GetWorkerByNameResponse>(new GetWorkerByNameRequest { WorkerId = context.Message.WorkerId });
        
        await hubContext.Clients.All.SendWorker(worker.Message.Name);

        logger.LogInformation("(NOTIFICATIONS - INTEGRATION) --- Worker assigned to todo {TodoId}", context.Message.TodoId);
    }
}