using BuildingBlocks.Common;

namespace BuildingBlocks.Behaviors;

public partial class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().GetRequestName() ?? "Unknown";

        LogHandlingCommand(requestName, request);

        var response = await next().ConfigureAwait(false);

        LogCommandHandled(requestName, response);

        return response;
    }


    [LoggerMessage(0, LogLevel.Information, "----- Handling command {CommandName} ({Command})")]
    private partial void LogHandlingCommand(string commandName, TRequest command);

    [LoggerMessage(1, LogLevel.Information, "----- Command {CommandName} handled - response: {Response}")]
    private partial void LogCommandHandled(string commandName, TResponse response);

    [LoggerMessage(2, LogLevel.Error, "----- Pipeline handling failed for request {Request} with error {Error}")]
    private partial void LogErrorInCommand(string request, string error);
}