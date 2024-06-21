using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Todos.Diagnostics;
using Todos.Infrastructure.Persistence;

namespace Todos.Features.Todo.Commands;

public class CompleteTodo : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/todos/{id}/complete",
                async (string id, ISender mediator, CancellationToken cancellationToken) =>
                {
                    var command = new Command
                    {
                        TodoId = id
                    };
                    
                    var result = await mediator.Send(command, cancellationToken);
                    return result.ToHttpResult();
                })
            .WithName(nameof(CompleteTodo))
            .WithTags(nameof(Domain.Todo))
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status404NotFound);
    }

    public sealed record Command : ICommand<Result>, IInvalidateCacheRequest
    {
        public string TodoId { get; internal set; } = null!;
        public string PrefixCacheKey => nameof(Domain.Todo);
    }

    internal class Handler(TodoDbContext db) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var todo = await db.Todos
                .SingleOrDefaultAsync(x => x.Id == request.TodoId, cancellationToken);
            
            if (todo == null)
            {
                return Result.Fail(new Error($"Todo {request.TodoId} not found"));
            }

            todo.CompleteTodo();

            await db.SaveChangesAsync(cancellationToken);
            
            Instrumentation.Todos.Completed.Add(1);

            return Result.Ok();
        }
    }
}