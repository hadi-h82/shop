using System.ComponentModel.DataAnnotations;

namespace Sevart.Api.Contracts.Products;

public sealed class AddProductImageRequest
{
    [Required]
    [MaxLength(2000)]
    public string Url { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; init; }

    public bool IsPrimary { get; init; }
}