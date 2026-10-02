using Microsoft.AspNetCore.Identity;

namespace Sevart.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    private readonly List<RefreshToken> _refreshTokens = [];

    public IReadOnlyCollection<RefreshToken> RefreshTokens
        => _refreshTokens;
    public string FullName { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private ApplicationUser()
    {
    }

    public ApplicationUser(
        string fullName,
        string email,
        string phoneNumber)
    {
        Id = Guid.NewGuid();

        SetFullName(fullName);

        Email = email.Trim().ToLowerInvariant();
        UserName = Email;
        PhoneNumber = phoneNumber.Trim();

        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string fullName,
        string phoneNumber)
    {
        SetFullName(fullName);

        PhoneNumber = phoneNumber.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRefreshToken(
    RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        _refreshTokens.Add(refreshToken);
    }

    private void SetFullName(string fullName)
    {
        var normalizedFullName = fullName.Trim();

        if (string.IsNullOrWhiteSpace(normalizedFullName))
        {
            throw new ArgumentException(
                "Full name is required.",
                nameof(fullName));
        }

        if (normalizedFullName.Length > 200)
        {
            throw new ArgumentException(
                "Full name cannot exceed 200 characters.",
                nameof(fullName));
        }

        FullName = normalizedFullName;
    }
}