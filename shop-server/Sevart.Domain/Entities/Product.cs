using Sevart.Domain.Common;

namespace Sevart.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
}
