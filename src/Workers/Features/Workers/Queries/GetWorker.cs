using BuildingBlocks.Cache;
using BuildingBlocks.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Workers.Infrastructure.Persistence;

namespace Workers.Features.Workers.Queries;

public class GetWorker : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/workers/{id}",
                async (string id, ISender mediator, CancellationToken cancellationToken) =>
                    await mediator.Send(new Query(id), cancellationToken))
            .WithName(nameof(GetWorker))
            .WithTags(nameof(Workers))
            .Produces<string>();
    }

    public record Query(string Id) : IQuery<string>, ICacheRequest
    {
        public string CacheKey { get; } = $"Worker:{Id}";
        public DateTime? AbsoluteExpirationRelativeToNow { get; }
    }

    internal class Handler(WorkersDbContext db) : IRequestHandler<Query,string>
    {
        public async Task<string> Handle(Query request, CancellationToken cancellationToken = default)
        {
           var worker=  await db.Workers
                .AsNoTracking()
                .Where(t => t.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);
            
            return worker?.Name ?? "Not Found";
        }
    }
}