namespace Sevart.Application.Abstractions.Identity;

public sealed record AuthError(
    string Code,
    string Description);