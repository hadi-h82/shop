namespace Sevart.Api.Contracts.Auth;

public sealed record AuthResponse(
    Guid UserId,
    string FullName,
    string Email,
    IReadOnlyCollection<string> Roles,
    string TokenType,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);