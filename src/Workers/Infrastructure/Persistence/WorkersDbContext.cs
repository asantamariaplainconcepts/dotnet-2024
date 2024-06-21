using Microsoft.EntityFrameworkCore;
using Workers.Domain;

namespace Workers.Infrastructure.Persistence;

public class WorkersDbContext(DbContextOptions<WorkersDbContext> options)
    : DbContext(options)
{
    public DbSet<Worker> Workers => Set<Worker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(nameof(Workers));

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkersDbContext).Assembly);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=Todos;User Id=sa;Password=Password123;");
        }
        else
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}