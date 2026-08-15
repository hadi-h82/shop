using Sevart.Domain.Common;

namespace Sevart.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public string Url { get; private set; } = string.Empty;

    public bool IsPrimary { get; private set; }

    public int DisplayOrder { get; private set; }
}