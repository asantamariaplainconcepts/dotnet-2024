using Aspirant.Hosting;
using AspireHost.Extensions;
using static AspireHost.Constants;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql", password: builder.CreateStablePassword("todos-password"))
    .WithHealthCheck()
    .WithDataVolume()
    .AddDatabase(Database);

var cache = builder.AddRedis("cache")
    .WithRedisCommander();

var queue = builder.AddRabbitMQ("queue")
    .WithManagementPlugin();

var seeder = builder.AddProject<Projects.DbSeeder>("db-seeder")
    .WithReference(sql, "SqlServer")
    .WaitFor(sql);

var api = builder.AddProject<Projects.Api>(Api)
    .WithReference(sql, "SqlServer")
    .WithReference(cache)
    .WithReference(queue)
    .WithExternalHttpEndpoints()
    .WaitFor(seeder);

builder.AddProject<Projects.Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();