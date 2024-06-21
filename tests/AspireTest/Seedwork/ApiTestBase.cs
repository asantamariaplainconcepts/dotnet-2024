namespace AspireTest.Seedwork;

[Collection(nameof(CollectionServerFixture))]
public class ApiTestBase(ApiServiceFixture given) : IAsyncLifetime
{
    protected ApiServiceFixture Given { get; set; } = given;

    public async Task InitializeAsync()
    {
        await Given.Reset();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}