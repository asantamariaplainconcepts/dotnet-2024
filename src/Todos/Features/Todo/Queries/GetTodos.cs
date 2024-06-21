using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Todos.Infrastructure.Persistence;

namespace Todos.Features.Todo.Queries;

public class GetTodos : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/todos",
                async (ISender mediator, CancellationToken cancellationToken) =>
                    await mediator.Send(new Query(), cancellationToken))
            .WithName(nameof(GetTodos))
            .WithTags(nameof(Domain.Todo))
            .Produces<Response[]>();
    }

    public record Query : IQuery<Response[]>;

    public record Response(string Id, string Title, bool IsCompleted, string? worker);

    internal class Handler(
        ReadOnlyTodoDbContext db,
        IRequestClient<GetWorkerByNameRequest> client,
        Diagnostics.Diagnostics diagnostics)
        : IRequestHandler<Query, Response[]>
    {
        public async Task<Response[]> Handle(Query request, CancellationToken cancellationToken = default)
        {
            diagnostics.GetTodoRequest();

            var data = await db.Todos
                .ToArrayAsync(cancellationToken);

            var filledTodos = await FillWorkerNames(data, cancellationToken);

            return filledTodos;
        }

        async Task<Response[]> FillWorkerNames(Domain.Todo[] data, CancellationToken cancellationToken)
        {
            var workerIds = data
                .Where(todo => !string.IsNullOrWhiteSpace(todo.WorkerId))
                .Select(todo => todo.WorkerId!)
                .Distinct()
                .ToArray();

            var workerNames = new Dictionary<string, string>();

            foreach (var workerId in workerIds)
            {
                var workerResponse = await client.GetResponse<GetWorkerByNameResponse>(new GetWorkerByNameRequest
                {
                    WorkerId = workerId
                }, cancellationToken);

                workerNames[workerId] = workerResponse.Message.Name;
            }

            var response = data.Select(todo => new Response(
                todo.Id,
                todo.Title,
                todo.Completed,
                todo.WorkerId != null ? workerNames[todo.WorkerId] : null
            )).ToArray();

            return response;
        }
    }
}