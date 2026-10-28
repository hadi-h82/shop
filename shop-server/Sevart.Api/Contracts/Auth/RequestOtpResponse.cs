namespace Sevart.Api.Contracts.Auth;

public sealed record RequestOtpResponse(
    Guid ChallengeId,
    string Message,
    int ExpiresInSeconds,
    int RetryAfterSeconds);