namespace Sevart.Application.Abstractions.Identity;

public interface ITokenService
{
    GeneratedAuthTokens GenerateTokens(
        Guid userId,
        string phoneNumber,
        string? email,
        string fullName,
        IReadOnlyCollection<string> roles);

    string HashRefreshToken(
    string refreshToken);
}