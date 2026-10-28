namespace Sevart.Infrastructure.Identity;

public sealed class OtpOptions
{
    public const string SectionName = "Otp";

    public int CodeLength { get; init; } = 6;

    public int ExpirationMinutes { get; init; } = 2;

    public int ResendCooldownSeconds { get; init; } = 90;

    public int MaxVerificationAttempts { get; init; } = 5;

    public int MaxSendsPerPhonePerHour { get; init; } = 3;

    public int MaxSendsPerPhonePerDay { get; init; } = 5;

    public int MaxSendsPerIpPerHour { get; init; } = 10;

    public int MaxSendsPerIpPerDay { get; init; } = 30;

    public string HashKey { get; init; } =
        string.Empty;
}