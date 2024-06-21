using Aspirant.Hosting.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Todos.Infrastructure.Persistence;
using static AspireHost.Constants;

namespace AspireTest.Seedwork;

public sealed class ApiServiceFixture : IAsyncLifetime
{
    private Respawner? _respawner;

    private DistributedApplication _app = null!;
    
    public HttpClient TodoClient { get; private set; } = new();

    internal IServiceProvider Services => _app.Services;

    private string? _connectionString;
    
    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireHost>();
        
        appHost.FixContentRoot();
        appHost.Services.AddResourceWatching();
        appHost.WithRandomVolumeNames();

        appHost.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(_connectionString));
        
        _app = await appHost.BuildAsync();

        await _app.StartAsync(waitForResourcesToStart: true);

        _connectionString = await _app.GetConnectionStringAsync(Database);

        if (_connectionString == null)
            throw new Exception("Connection string not found");

        _respawner = await Respawner.CreateAsync(_connectionString);

        TodoClient = _app.CreateHttpClient("todo-api");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task Reset()
    {
        if (_respawner != null)
            await _respawner.ResetAsync(_connectionString!);
    }

    public async Task ExecuteDbContextAsync(Func<TodoDbContext, Task> function)
    {
        var factory = _app.Services.GetRequiredService<IServiceScopeFactory>();

        using var scope = factory.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<TodoDbContext>();

        await function(context);
    }
}