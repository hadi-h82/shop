namespace Sevart.Application.Abstractions.Identity;

public sealed record RefreshSessionCommand(
    string RefreshToken,
    string? IpAddress);