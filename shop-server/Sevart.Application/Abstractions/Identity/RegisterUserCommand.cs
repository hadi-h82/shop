namespace Sevart.Application.Abstractions.Identity;

public sealed record RegisterUserCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    string? IpAddress);