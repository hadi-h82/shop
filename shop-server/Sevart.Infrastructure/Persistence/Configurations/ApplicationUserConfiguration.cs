using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevart.Infrastructure.Identity;

namespace Sevart.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(
        EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(256);

        builder.HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("EmailIndex")
            .IsUnique()
            .HasFilter("[NormalizedEmail] IS NOT NULL");

        builder.Property(x => x.UserName)
            .HasMaxLength(256);

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(256);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);
    }
}