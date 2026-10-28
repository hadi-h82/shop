namespace Sevart.Infrastructure.Identity;

public class RefreshToken
{
    public Guid Id { get; private set; }

    public string TokenHash { get; private set; }
        = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string? CreatedByIp { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public string? RevokedByIp { get; private set; }

    public string? ReplacedByTokenHash { get; private set; }

    public Guid UserId { get; private set; }

    public ApplicationUser User { get; private set; }
        = null!;

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsActive =>
        RevokedAt is null &&
        !IsExpired;

    private RefreshToken()
    {
    }

    public RefreshToken(
        string tokenHash,
        DateTime expiresAt,
        string? createdByIp)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException(
                "Token hash is required.",
                nameof(tokenHash));
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Expiration time must be in the future.",
                nameof(expiresAt));
        }

        Id = Guid.NewGuid();
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        CreatedByIp = NormalizeIp(createdByIp);
    }

    public void Revoke(
        string? revokedByIp,
        string? replacedByTokenHash = null)
    {
        if (!IsActive)
        {
            return;
        }

        RevokedAt = DateTime.UtcNow;
        RevokedByIp = NormalizeIp(revokedByIp);

        ReplacedByTokenHash =
            string.IsNullOrWhiteSpace(replacedByTokenHash)
                ? null
                : replacedByTokenHash;
    }

    private static string? NormalizeIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return null;
        }

        return ipAddress.Trim();
    }
}