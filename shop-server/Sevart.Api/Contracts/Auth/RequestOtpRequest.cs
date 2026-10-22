using System.ComponentModel.DataAnnotations;

namespace Sevart.Api.Contracts.Auth;

public sealed class RequestOtpRequest
{
    [Required]
    [StringLength(
        20,
        MinimumLength = 10)]
    public string PhoneNumber { get; init; } =
        string.Empty;
}