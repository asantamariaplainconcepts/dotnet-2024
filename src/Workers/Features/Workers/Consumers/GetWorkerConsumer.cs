using Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Workers.Features.Workers.Queries;

namespace Workers.Features.Workers.Consumers;

public class GetWorkerConsumer(ISender sender, ILogger<GetWorkerConsumer> logger)
    : IConsumer<GetWorkerByNameRequest>
{
    public async Task Consume(ConsumeContext<GetWorkerByNameRequest> context)
    {
        logger.LogInformation("(Workers) Getting worker by id {WorkerId}", context.Message.WorkerId);

        var name = await sender.Send(new GetWorker.Query(context.Message.WorkerId));

        var response = new GetWorkerByNameResponse
        {
            WorkerId = context.Message.WorkerId,
            Name = name
        };
        
        logger.LogInformation("(Workers) Returning worker {WorkerId} with name {Name}", response.WorkerId, response.Name);

        await context.RespondAsync(response);
    }
}