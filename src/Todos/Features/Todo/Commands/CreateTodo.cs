using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Todos.Diagnostics;
using Todos.Infrastructure.Persistence;

namespace Todos.Features.Todo.Commands;

public class CreateTodo : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/todos",
                async (Command command, ISender mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName(nameof(CreateTodo))
            .WithTags(nameof(Domain.Todo))
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    public sealed record Command : ICommand<Result<string>>, IInvalidateCacheRequest
    {
        public string Title { get; set; } = string.Empty;
        public string PrefixCacheKey => nameof(Domain.Todo);
    }

    internal class Handler(TodoDbContext db) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var todo = new Domain.Todo(request.Title);
            
            await db.Todos.AddAsync(todo, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);
            
            Instrumentation.Todos.Created.Add(1);

            return Result<string>.Ok(todo.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.Title).NotEmpty();
        }
    }
}