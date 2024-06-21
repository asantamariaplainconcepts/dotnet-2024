using MediatR;

namespace Contracts;

public record TodoCompletedEvent(string Id, string Title, string Worker) : INotification;