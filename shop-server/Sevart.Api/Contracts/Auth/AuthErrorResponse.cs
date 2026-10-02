namespace Sevart.Api.Contracts.Auth;

public sealed record AuthErrorResponse(
    IReadOnlyCollection<AuthErrorItem> Errors);

public sealed record AuthErrorItem(
    string Code,
    string Message);