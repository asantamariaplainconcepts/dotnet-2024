namespace Contracts;

public record GetWorkerByNameRequest
{
    public required string WorkerId { get; init; }
}

public record GetWorkerByNameResponse
{
    public required string WorkerId { get; init; }
    
    public required string Name { get; init; }
}