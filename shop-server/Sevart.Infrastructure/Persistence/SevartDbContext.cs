using Microsoft.EntityFrameworkCore;
using Sevart.Domain.Entities;
using Sevart.Domain.Common;

namespace Sevart.Infrastructure.Persistence;

public class SevartDbContext : DbContext
{
    public SevartDbContext(
        DbContextOptions<SevartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<ProductOption> ProductOptions => Set<ProductOption>();

    public DbSet<ProductOptionValue> ProductOptionValues
        => Set<ProductOptionValue>();

    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseAuditableEntity>();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedAt)
                    .CurrentValue = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedAt)
                    .CurrentValue = now;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SevartDbContext).Assembly);
    }
}