namespace Sevart.Api.Contracts.Auth;

public sealed record RequestOtpErrorResponse(
    IReadOnlyCollection<AuthErrorItem> Errors,
    int RetryAfterSeconds);