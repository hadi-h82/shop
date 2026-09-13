using Sevart.Domain.Enums;

namespace Sevart.Api.Contracts.Products;

public class ProductResponse
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public List<ProductOptionResponse> Options { get; set; } = [];
}


public class ProductOptionResponse
{
    public int Id { get; set; }

    public int ProductOptionDefinitionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProductOptionInputType InputType { get; set; }

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public List<ProductOptionValueResponse> Values { get; set; } = [];
}


public class ProductOptionValueResponse
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }

    public string? ColorCode { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }
}