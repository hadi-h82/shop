using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevart.Infrastructure.Identity;

namespace Sevart.Infrastructure.Persistence.Configurations;

public class OtpChallengeConfiguration
    : IEntityTypeConfiguration<OtpChallenge>
{
    public void Configure(
        EntityTypeBuilder<OtpChallenge> builder)
    {
        builder.ToTable("OtpChallenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CodeHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Purpose)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedByIp)
            .HasMaxLength(45);

        builder.Property(x => x.FailedAttemptCount)
            .IsRequired();

        builder.Property(x => x.MaxAttempts)
            .IsRequired();

        builder.Property(x => x.UsedAt);

        builder.Property(x => x.InvalidatedAt);

        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsActive);

        builder.HasIndex(
                x => new
                {
                    x.PhoneNumber,
                    x.Purpose,
                    x.CreatedAt
                })
            .HasDatabaseName(
                "IX_OtpChallenges_PhoneNumber_Purpose_CreatedAt");

        builder.HasIndex(
                x => new
                {
                    x.CreatedByIp,
                    x.CreatedAt
                })
            .HasDatabaseName(
                "IX_OtpChallenges_CreatedByIp_CreatedAt");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName(
                "IX_OtpChallenges_ExpiresAt");
    }
}