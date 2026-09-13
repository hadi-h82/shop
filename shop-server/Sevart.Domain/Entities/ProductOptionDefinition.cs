using Sevart.Domain.Common;
using Sevart.Domain.Enums;

namespace Sevart.Domain.Entities;

public class ProductOptionDefinition : BaseAuditableEntity
{
    private ProductOptionDefinition()
    {
    }

    public ProductOptionDefinition(
        string name,
        string slug,
        ProductOptionInputType inputType,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Option definition name cannot be empty.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Option definition slug cannot be empty.",
                nameof(slug));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = name;
        Slug = slug;
        InputType = inputType;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public ProductOptionInputType InputType { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(
        string name,
        string slug,
        ProductOptionInputType inputType,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Option definition name cannot be empty.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Option definition slug cannot be empty.",
                nameof(slug));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = name;
        Slug = slug;
        InputType = inputType;
        DisplayOrder = displayOrder;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}