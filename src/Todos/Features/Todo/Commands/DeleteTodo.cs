using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Todos.Infrastructure.Persistence;

namespace Todos.Features.Todo.Commands;

public class DeleteTodo : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/todos/{id}",
                async (string id, ISender mediator, CancellationToken cancellationToken) =>
                {
                    var request = new Command
                    {
                        TodoId = id
                    };

                    var result = await mediator.Send(request, cancellationToken);

                    return  result.ToHttpResult();
                })
            .WithName(nameof(DeleteTodo))
            .WithTags(nameof(Domain.Todo))
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    public sealed record Command : ICommand<Result>, IInvalidateCacheRequest
    {
        public string TodoId { get; set; } = string.Empty;
        public string PrefixCacheKey => nameof(Domain.Todo);
    }

    internal class CommandHandler(TodoDbContext db) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var todo = await db.Todos
                .SingleOrDefaultAsync(x => x.Id == request.TodoId, cancellationToken);

            if (todo == null)
            {
                return Result.Ok();
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.TodoId).NotEmpty();
        }
    }
}