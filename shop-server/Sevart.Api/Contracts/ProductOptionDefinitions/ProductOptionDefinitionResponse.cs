using Sevart.Domain.Enums;

namespace Sevart.Api.Contracts.ProductOptionDefinitions;

public class ProductOptionDefinitionResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public ProductOptionInputType InputType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}