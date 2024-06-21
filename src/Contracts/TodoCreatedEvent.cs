using MediatR;

namespace Contracts;

public sealed record TodoCreatedEvent(string TodoId) : INotification;