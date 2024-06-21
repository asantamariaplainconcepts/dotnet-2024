using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workers.Domain;

namespace Workers.Infrastructure.Persistence.Configuration
{
    internal class WorkerEntityTypeConfiguration
        : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.ToTable("workers");
          
            builder.HasKey(t => t.Id);
           
            builder.Property(t => t.Name)
                .IsRequired();
        }
    }
}
