using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todos.Domain;

namespace Todos.Infrastructure.Persistence.Configuration
{
    internal class TodoEntityTypeConfiguration
        : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.ToTable("todos");
          
            builder.HasKey(t => t.Id);
           
            builder.Property(t => t.Title)
                .IsRequired();
            
            builder.Property(t => t.WorkerId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(t => t.Completed)
                .HasColumnType("BIT")
                .IsRequired();
        }
    }
}
