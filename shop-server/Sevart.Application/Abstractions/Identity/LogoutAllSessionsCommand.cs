namespace Sevart.Application.Abstractions.Identity;

public sealed record LogoutAllSessionsCommand(
    Guid UserId,
    string? IpAddress);