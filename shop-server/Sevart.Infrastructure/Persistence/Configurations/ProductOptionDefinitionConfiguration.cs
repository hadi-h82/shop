using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevart.Domain.Entities;

namespace Sevart.Infrastructure.Persistence.Configurations;

public class ProductOptionDefinitionConfiguration
    : IEntityTypeConfiguration<ProductOptionDefinition>
{
    public void Configure(
        EntityTypeBuilder<ProductOptionDefinition> builder)
    {
        builder.ToTable("ProductOptionDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.InputType)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}