namespace Sevart.Application.Abstractions.Identity;

public sealed record VerifyOtpCommand(
    Guid ChallengeId,
    string Code,
    string? IpAddress);