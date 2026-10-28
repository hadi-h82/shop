using Microsoft.AspNetCore.Identity;
using Sevart.Application.Abstractions.Identity;

namespace Sevart.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    private readonly List<RefreshToken> _refreshTokens = [];

    public IReadOnlyCollection<RefreshToken> RefreshTokens =>
        _refreshTokens;

    public string FullName { get; private set; } =
        string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private ApplicationUser()
    {
    }

    // Constructor used for mobile and OTP registration.
    public ApplicationUser(
        string phoneNumber)
    {
        Id = Guid.NewGuid();

        SetPhoneNumber(phoneNumber);

        CreatedAt = DateTime.UtcNow;
    }


    public void UpdateProfile(
        string fullName,
        string? email)
    {
        SetFullName(fullName);
        SetEmail(email);

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRefreshToken(
        RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(
            refreshToken);

        _refreshTokens.Add(refreshToken);
    }

    private void SetFullName(
        string fullName)
    {
        var normalizedFullName =
            fullName.Trim();

        if (string.IsNullOrWhiteSpace(
                normalizedFullName))
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

    private void SetPhoneNumber(
        string phoneNumber)
    {
        if (!IranianMobileNumberNormalizer.TryNormalize(
                phoneNumber,
                out var normalizedPhoneNumber))
        {
            throw new ArgumentException(
                "A valid Iranian mobile number is required.",
                nameof(phoneNumber));
        }

        PhoneNumber = normalizedPhoneNumber;
        UserName = normalizedPhoneNumber;
    }

    private void SetEmail(
        string? email)
    {
        Email =
            string.IsNullOrWhiteSpace(email)
                ? null
                : email
                    .Trim()
                    .ToLowerInvariant();
    }
}