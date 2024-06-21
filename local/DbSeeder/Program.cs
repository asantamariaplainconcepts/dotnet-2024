using DbSeeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServer");

if(connectionString == null)
{
    throw new Exception("Connection string not found");
}

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<DbMigrate>>();

var migrator = new DbMigrate(logger);

migrator.Migrate(connectionString);