using Sevart.Domain.Enums;

namespace Sevart.Api.Contracts.Products;

public class AdminProductListItemResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public ProductStatus Status { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }
}