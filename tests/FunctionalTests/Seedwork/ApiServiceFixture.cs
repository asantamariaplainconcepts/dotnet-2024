using Api;
using BuildingBlocks.Cache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Todos.Infrastructure.Persistence;
using Workers.Infrastructure.Persistence;

namespace FunctionalTests.Seedwork;

public sealed class ApiServiceFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner? _respawner;

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    

    public HttpClient Client { get; private set; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:SqlServer", _msSqlContainer.GetConnectionString()),
                new KeyValuePair<string, string?>("ConnectionStrings:redis", _redisContainer.GetConnectionString())
            });
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        var dbContext = new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString())
            .Options);

        await dbContext.Database.MigrateAsync();


        var workers = new WorkersDbContext(new DbContextOptionsBuilder<WorkersDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString())
            .Options);

        await workers.Database.MigrateAsync();

        _respawner = await Respawner.CreateAsync(_msSqlContainer.GetConnectionString());

        Client = Server.CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }

    public async Task Reset()
    {
        if (_respawner != null)
            await _respawner.ResetAsync(_msSqlContainer.GetConnectionString()!);
    }

    public async Task ExecuteDbContextAsync(Func<TodoDbContext, Task> function)
    {
        var factory = Services.GetRequiredService<IServiceScopeFactory>();

        using var scope = factory.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<TodoDbContext>();

        await function(context);
    }
}