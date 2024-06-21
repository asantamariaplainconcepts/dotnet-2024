namespace BuildingBlocks.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess == false)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok(result.Value);
    }
    
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess == false)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok();
    }
}