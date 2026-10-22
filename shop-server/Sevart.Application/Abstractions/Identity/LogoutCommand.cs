namespace Sevart.Application.Abstractions.Identity;

public sealed record LogoutCommand(
    string? RefreshToken,
    string? IpAddress);