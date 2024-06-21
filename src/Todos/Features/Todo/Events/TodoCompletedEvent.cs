using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Todos.Features.Todo.Events;

public class TodoCompletedEventHandler(IPublishEndpoint publishEndpoint, ILogger<TodoCompletedEventHandler> logger)
    : INotificationHandler<TodoCompletedEvent>
{
    public async Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("(TODOS - DOMAIN EVENT) --- Todo {TodoId} has been completed", notification.Id);

        await publishEndpoint.Publish(notification, cancellationToken);
    }
}