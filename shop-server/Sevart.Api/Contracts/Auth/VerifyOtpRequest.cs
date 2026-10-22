using System.ComponentModel.DataAnnotations;

namespace Sevart.Api.Contracts.Auth;

public sealed class VerifyOtpRequest
{
    public Guid ChallengeId { get; init; }

    [Required]
    [StringLength(
        8,
        MinimumLength = 4)]
    public string Code { get; init; } =
        string.Empty;
}