namespace Sevart.Application.Abstractions.Identity;

public interface ITokenService
{
    GeneratedAuthTokens GenerateTokens(
        Guid userId,
        string email,
        string fullName,
        IReadOnlyCollection<string> roles);
}