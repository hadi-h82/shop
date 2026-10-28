using Sevart.Application.Abstractions.Identity;

namespace Sevart.Infrastructure.Identity;

public class OtpChallenge
{
    public Guid Id { get; private set; }

    public string PhoneNumber { get; private set; } =
        string.Empty;

    public string CodeHash { get; private set; } =
        string.Empty;

    public OtpPurpose Purpose { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string? CreatedByIp { get; private set; }

    public int FailedAttemptCount { get; private set; }

    public int MaxAttempts { get; private set; }

    public DateTime? UsedAt { get; private set; }

    public DateTime? InvalidatedAt { get; private set; }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsActive =>
        UsedAt is null &&
        InvalidatedAt is null &&
        !IsExpired &&
        FailedAttemptCount < MaxAttempts;

    private OtpChallenge()
    {
    }

    public OtpChallenge(
        Guid id,
        string phoneNumber,
        string codeHash,
        OtpPurpose purpose,
        DateTime expiresAt,
        int maxAttempts,
        string? createdByIp)
    {


        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "OTP challenge ID is required.",
                nameof(id));
        }

        if (!IranianMobileNumberNormalizer.TryNormalize(
                phoneNumber,
                out var normalizedPhoneNumber))
        {
            throw new ArgumentException(
                "A valid Iranian mobile number is required.",
                nameof(phoneNumber));
        }

        if (string.IsNullOrWhiteSpace(codeHash))
        {
            throw new ArgumentException(
                "OTP code hash is required.",
                nameof(codeHash));
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Expiration time must be in the future.",
                nameof(expiresAt));
        }

        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts),
                "Maximum attempts must be greater than zero.");
        }

        Id = id;
        PhoneNumber = normalizedPhoneNumber;
        CodeHash = codeHash;
        Purpose = purpose;
        ExpiresAt = expiresAt;
        MaxAttempts = maxAttempts;
        CreatedByIp = NormalizeIp(createdByIp);
        CreatedAt = DateTime.UtcNow;
    }

    public void RegisterFailedAttempt()
    {
        if (!IsActive)
        {
            return;
        }

        FailedAttemptCount++;

        if (FailedAttemptCount >= MaxAttempts)
        {
            InvalidatedAt = DateTime.UtcNow;
        }
    }

    public void MarkAsUsed()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException(
                "OTP challenge is not active.");
        }

        UsedAt = DateTime.UtcNow;
    }

    public void Invalidate()
    {
        if (UsedAt is not null ||
            InvalidatedAt is not null)
        {
            return;
        }

        InvalidatedAt = DateTime.UtcNow;
    }

    private static string? NormalizeIp(
        string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return null;
        }

        return ipAddress.Trim();
    }
}