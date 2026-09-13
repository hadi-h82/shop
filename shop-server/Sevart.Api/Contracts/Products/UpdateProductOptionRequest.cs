namespace Sevart.Api.Contracts.Products;

public class UpdateProductOptionRequest
{
    public int? Id { get; set; }

    public int ProductOptionDefinitionId { get; set; }

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public List<UpdateProductOptionValueRequest> Values { get; set; } = [];
}