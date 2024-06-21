using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Workers.Infrastructure.Persistence
{
    internal class WorkerssDbContextDesignTimeFactory : IDesignTimeDbContextFactory<WorkersDbContext>
    {
        public WorkersDbContext CreateDbContext(string[] args)
        {
            // just used it to add migrations on this project because is not 
            // a executable, is not used on runtime

            var optionsBuilder = new DbContextOptionsBuilder<WorkersDbContext>();
            return new WorkersDbContext(optionsBuilder.Options);
        }
    }
}