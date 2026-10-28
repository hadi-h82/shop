namespace Sevart.Application.Abstractions.Identity;

public interface IOtpMessageSender
{
    Task SendSignInCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);
}