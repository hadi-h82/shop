using Microsoft.EntityFrameworkCore;
using Sevart.Domain.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SevartDbContext).Assembly);
    }
}