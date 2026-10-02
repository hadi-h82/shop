namespace Sevart.Application.Abstractions.Identity;

public sealed record RegisterUserResult(
    bool Succeeded,
    AuthSession? Session,
    IReadOnlyCollection<AuthError> Errors)
{
    public static RegisterUserResult Success(
        AuthSession session)
    {
        return new RegisterUserResult(
            true,
            session,
            []);
    }

    public static RegisterUserResult Failure(
        IReadOnlyCollection<AuthError> errors)
    {
        return new RegisterUserResult(
            false,
            null,
            errors);
    }
}