namespace Sevart.Application.Abstractions.Identity;

public interface IOtpCodeService
{
    string GenerateCode();

    string ComputeHash(
        Guid challengeId,
        string code);

    bool VerifyCode(
        Guid challengeId,
        string code,
        string expectedHash);
}