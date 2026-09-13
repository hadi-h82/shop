namespace Sevart.Api.Contracts.Products;

public class UpdateProductOptionValueRequest
{
    public int? Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }

    public string? ColorCode { get; set; }

    public int DisplayOrder { get; set; }
}