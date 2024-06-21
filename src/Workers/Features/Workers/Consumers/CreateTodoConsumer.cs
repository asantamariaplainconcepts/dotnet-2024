using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Workers.Infrastructure.Persistence;

namespace Workers.Features.Workers.Consumers;

public class CreateTodoConsumer(
    ILogger<CreateTodoConsumer> logger,
    WorkersDbContext dbContext,
    IPublishEndpoint publishEndpoint) : IConsumer<TodoCreatedEvent>
{
    public async Task Consume(ConsumeContext<TodoCreatedEvent> context)
    {
        logger.LogInformation("(WORKERS - INTEGRATION) --- Assigning worker to todo {TodoId}",
            context.Message.TodoId);

        var workers = dbContext.Workers.ToList();

        var random = new Random();

        var worker = workers[random.Next(workers.Count)];

        var assignWorker = new TodoWorkerAssigmentEvent(context.Message.TodoId, worker.Id);

        logger.LogInformation("(WORKERS - INTEGRATION) --- Worker {name} assigned to todo {TodoId}", worker.Name,
            context.Message.TodoId);
        
        await Task.Delay(5000);

        await publishEndpoint.Publish(assignWorker);
    }
}