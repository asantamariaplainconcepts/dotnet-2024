namespace BuildingBlocks.Common;

public static class GetTypeName
{
    public static string? GetRequestName(this Type type)
    {
        return type.FullName?
            .Split('.')
            .Last()
            .Replace('+', '_');
    }
}