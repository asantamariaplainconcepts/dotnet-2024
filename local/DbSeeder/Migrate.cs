using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;

namespace DbSeeder;

public class DbMigrate(ILogger logger)
{
    public DatabaseUpgradeResult Migrate(string connectionString)
    {
        while (true)
        {
            try
            {
                EnsureDatabase.For.SqlDatabase(connectionString);
                break;
            }
            catch
            {
                logger.LogInformation("Waiting for SQL Server to be ready...");
                Thread.Sleep(1000);
            }
        }
        
        logger.LogInformation("Migrating database");

        var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "migrations");

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(scriptPath)
            .WithTransactionPerScript()
            .WithExecutionTimeout(TimeSpan.FromMinutes(5))
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        
        if (result.Successful)
        {
            logger.LogInformation("Database migration successful");
        }
        else
        {
            logger.LogError("Database migration failed");
            return result;
        }
        
        logger.LogInformation("Database migration completed");
        
            logger.LogInformation("Seeding database");
        
        var seedsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seeds");

        var seeder = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(seedsPath)
            .WithTransactionPerScript()
            .WithExecutionTimeout(TimeSpan.FromMinutes(5))
            .LogToConsole()
            .Build();

        
        result = seeder.PerformUpgrade();
        
        if (result.Successful)
        {
            logger.LogInformation("Database seeding successful");
        }
        else
        {
            logger.LogError("Database seeding failed");
            return result;
        }

        return result;
    }
}