namespace Sevart.Application.Abstractions.Identity;

public sealed record GeneratedAuthTokens(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    string RefreshTokenHash,
    DateTime RefreshTokenExpiresAt);