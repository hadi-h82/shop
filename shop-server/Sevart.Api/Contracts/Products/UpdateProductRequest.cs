namespace Sevart.Api.Contracts.Products;

public class UpdateProductRequest
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DisplayOrder { get; set; }

    public List<UpdateProductOptionRequest> Options { get; set; } = [];
}