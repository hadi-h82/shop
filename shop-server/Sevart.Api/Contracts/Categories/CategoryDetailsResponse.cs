using Sevart.Api.Contracts.Products;

namespace Sevart.Api.Contracts.Categories;

public class CategoryDetailsResponse
{
    public CategoryResponse Category { get; set; } = null!;

    public List<ProductResponse> Products { get; set; } = [];
}