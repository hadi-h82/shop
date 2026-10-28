namespace Sevart.Application.Abstractions.Identity;

public sealed record RequestOtpResult(
    bool Succeeded,
    Guid? ChallengeId,
    int? ExpiresInSeconds,
    int RetryAfterSeconds,
    IReadOnlyCollection<AuthError> Errors)
{
    public static RequestOtpResult Success(
        Guid challengeId,
        int expiresInSeconds,
        int retryAfterSeconds)
    {
        return new RequestOtpResult(
            Succeeded: true,
            ChallengeId: challengeId,
            ExpiresInSeconds: expiresInSeconds,
            RetryAfterSeconds: retryAfterSeconds,
            Errors: []);
    }

    public static RequestOtpResult Failure(
        AuthError error,
        int retryAfterSeconds = 0)
    {
        return new RequestOtpResult(
            Succeeded: false,
            ChallengeId: null,
            ExpiresInSeconds: null,
            RetryAfterSeconds: retryAfterSeconds,
            Errors: [error]);
    }
}