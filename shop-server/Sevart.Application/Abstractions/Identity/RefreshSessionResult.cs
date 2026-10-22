namespace Sevart.Application.Abstractions.Identity;

public sealed record RefreshSessionResult(
    bool Succeeded,
    AuthSession? Session,
    IReadOnlyCollection<AuthError> Errors)
{
    public static RefreshSessionResult Success(
        AuthSession session)
    {
        return new RefreshSessionResult(
            Succeeded: true,
            Session: session,
            Errors: []);
    }

    public static RefreshSessionResult Failure(
        AuthError error)
    {
        return new RefreshSessionResult(
            Succeeded: false,
            Session: null,
            Errors: [error]);
    }
}