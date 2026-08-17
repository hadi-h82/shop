using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevart.Domain.Entities;

namespace Sevart.Infrastructure.Persistence.Configurations;

public class ProductOptionValueConfiguration
    : IEntityTypeConfiguration<ProductOptionValue>
{
    public void Configure(
        EntityTypeBuilder<ProductOptionValue> builder)
    {
        builder.ToTable("ProductOptionValues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PriceAdjustment)
            .HasPrecision(18, 2);

        builder.Property(x => x.ColorCode)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.HasOne(x => x.ProductOption)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.ProductOptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}