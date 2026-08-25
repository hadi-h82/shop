using Sevart.Domain.Enums;

namespace Sevart.Api.Contracts.Products;

public class AdminProductResponse
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public ProductStatus Status { get; set; }

    public int DisplayOrder { get; set; }

    public List<AdminProductImageResponse> Images { get; set; } = [];

    public List<AdminProductOptionResponse> Options { get; set; } = [];
}

public class AdminProductImageResponse
{
    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }
}

public class AdminProductOptionResponse
{
    public int Id { get; set; }

    public int ProductOptionDefinitionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProductOptionInputType InputType { get; set; }

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public List<AdminProductOptionValueResponse> Values { get; set; } = [];
}

public class AdminProductOptionValueResponse
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }

    public string? ColorCode { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }
}