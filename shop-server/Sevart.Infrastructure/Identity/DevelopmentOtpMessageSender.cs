using Microsoft.Extensions.Logging;
using Sevart.Application.Abstractions.Identity;

namespace Sevart.Infrastructure.Identity;

public sealed class DevelopmentOtpMessageSender
    : IOtpMessageSender
{
    private readonly ILogger<DevelopmentOtpMessageSender>
        _logger;

    public DevelopmentOtpMessageSender(
        ILogger<DevelopmentOtpMessageSender> logger)
    {
        _logger = logger;
    }

    public Task SendSignInCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogWarning(
            "Development OTP for {PhoneNumber}: {OtpCode}",
            phoneNumber,
            code);

        return Task.CompletedTask;
    }
}