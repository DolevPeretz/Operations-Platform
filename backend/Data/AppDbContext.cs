
using Microsoft.EntityFrameworkCore;
using Operations_Platform_Assignment.Models;

namespace Operations_Platform_Assignment.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<OperationExecution> OperationExecutions
        => Set<OperationExecution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OperationExecution>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OperationId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.InputJson)
                .IsRequired();

            entity.Property(e => e.ResultJson);

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(1000);

            entity.Property(e => e.ExecutedAtUtc)
                .IsRequired();

            entity.HasIndex(e => new
            {
                e.OperationId,
                e.ExecutedAtUtc
            });
        });
    }
}
