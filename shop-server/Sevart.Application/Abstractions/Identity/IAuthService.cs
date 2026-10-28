namespace Sevart.Application.Abstractions.Identity;

public interface IAuthService
{

    Task<RequestOtpResult> RequestOtpAsync(
    RequestOtpCommand command,
    CancellationToken cancellationToken = default);

    Task<VerifyOtpResult> VerifyOtpAsync(
    VerifyOtpCommand command,
    CancellationToken cancellationToken = default);

    Task<RefreshSessionResult> RefreshSessionAsync(
    RefreshSessionCommand command,
    CancellationToken cancellationToken = default);

    Task LogoutAsync(
    LogoutCommand command,
    CancellationToken cancellationToken = default);

    Task LogoutAllSessionsAsync(
    LogoutAllSessionsCommand command,
    CancellationToken cancellationToken = default);


}