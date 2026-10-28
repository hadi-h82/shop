namespace Sevart.Application.Abstractions.Identity;

public sealed record RequestOtpCommand(
    string PhoneNumber,
    string? IpAddress);