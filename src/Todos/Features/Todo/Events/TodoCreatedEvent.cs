using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Todos.Features.Todo.Events;

public class TodoCreatedEventHandler(IPublishEndpoint publishEndpoint, ILogger<TodoCreatedEventHandler> logger) : INotificationHandler<TodoCreatedEvent>
{
    public async Task Handle(TodoCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("(TODOS - DOMAIN) Created todo {TodoId}", notification.TodoId);
        
        await publishEndpoint.Publish(notification, cancellationToken);
    }
}