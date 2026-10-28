namespace Sevart.Application.Abstractions.Identity;

public sealed record AuthSession(
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string? Email,
    IReadOnlyCollection<string> Roles,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);