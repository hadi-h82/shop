namespace Sevart.Application.Abstractions.Identity;

public sealed record VerifyOtpResult(
    bool Succeeded,
    AuthSession? Session,
    IReadOnlyCollection<AuthError> Errors)
{
    public static VerifyOtpResult Success(
        AuthSession session)
    {
        return new VerifyOtpResult(
            Succeeded: true,
            Session: session,
            Errors: []);
    }

    public static VerifyOtpResult Failure(
        AuthError error)
    {
        return new VerifyOtpResult(
            Succeeded: false,
            Session: null,
            Errors: [error]);
    }
}