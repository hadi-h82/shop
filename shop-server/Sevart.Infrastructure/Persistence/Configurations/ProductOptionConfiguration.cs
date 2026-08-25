using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevart.Domain.Entities;

namespace Sevart.Infrastructure.Persistence.Configurations;

public class ProductOptionConfiguration
    : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(
        EntityTypeBuilder<ProductOption> builder)
    {
        builder.ToTable("ProductOptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Product -> ProductOptions
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductOptionDefinition -> ProductOptions
        builder.HasOne(x => x.ProductOptionDefinition)
            .WithMany()
            .HasForeignKey(x => x.ProductOptionDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}