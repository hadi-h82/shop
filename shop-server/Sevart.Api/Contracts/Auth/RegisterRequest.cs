using System.ComponentModel.DataAnnotations;

namespace Sevart.Api.Contracts.Auth;

public sealed class RegisterRequest
{
    [Required]
    [StringLength(
        200,
        MinimumLength = 3)]
    public string FullName { get; init; }
        = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; }
        = string.Empty;

    [Required]
    [RegularExpression(
        @"^09\d{9}$",
        ErrorMessage =
            "Phone number must be a valid Iranian mobile number.")]
    public string PhoneNumber { get; init; }
        = string.Empty;

    [Required]
    [StringLength(
        100,
        MinimumLength = 8)]
    public string Password { get; init; }
        = string.Empty;
}