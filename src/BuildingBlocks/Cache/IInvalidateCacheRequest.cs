namespace BuildingBlocks.Cache;

public interface IInvalidateCacheRequest
{
    public string PrefixCacheKey { get; }
}