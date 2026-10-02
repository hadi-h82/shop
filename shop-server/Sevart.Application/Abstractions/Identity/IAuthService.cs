namespace Sevart.Application.Abstractions.Identity;

public interface IAuthService
{
    Task<RegisterUserResult> RegisterAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default);
}