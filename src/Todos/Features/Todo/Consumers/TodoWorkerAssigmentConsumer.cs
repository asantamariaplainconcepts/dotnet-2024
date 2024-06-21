using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Todos.Infrastructure.Persistence;

namespace Todos.Features.Todo.Consumers;

internal class TodoWorkerAssigmentConsumer(TodoDbContext todoDbContext, ILogger<TodoWorkerAssigmentEvent> logger) : IConsumer<TodoWorkerAssigmentEvent>
{
    public async Task Consume(ConsumeContext<TodoWorkerAssigmentEvent> context)
    {
        logger.LogInformation("(TODOS - INTEGRATION) --- Worker assigned to todo {TodoId}", context.Message.TodoId);
        
        var todo = await todoDbContext.Todos.SingleAsync(x => x.Id == context.Message.TodoId);
        
        todo.AssignWork(context.Message.WorkerId);
        
        logger.LogInformation("(TODOS - INTEGRATION) --- Worker assigned, todo completed {TodoId}", context.Message.TodoId);

        await todoDbContext.SaveChangesAsync();
    }
}