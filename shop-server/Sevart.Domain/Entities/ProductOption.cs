using Sevart.Domain.Common;

namespace Sevart.Domain.Entities;

public class ProductOption : BaseEntity
{
    private readonly List<ProductOptionValue> _values = new();

    // EF Core
    private ProductOption()
    {
    }

    public ProductOption(
        Product product,
        ProductOptionDefinition definition,
        bool isRequired,
        int displayOrder)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (!definition.IsActive)
        {
            throw new InvalidOperationException(
                "Inactive product option definition cannot be used.");
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Product = product;
        ProductOptionDefinition = definition;

        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public int ProductId { get; private set; }

    public Product Product { get; private set; } = null!;


    public int ProductOptionDefinitionId { get; private set; }

    public ProductOptionDefinition ProductOptionDefinition { get; private set; } = null!;


    public bool IsRequired { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;


    public IReadOnlyCollection<ProductOptionValue> Values
        => _values.AsReadOnly();


    public void AddValue(
        string label,
        string value,
        decimal priceAdjustment,
        string? colorCode,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException(
                "Product option value label cannot be empty.",
                nameof(label));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Product option value cannot be empty.",
                nameof(value));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        var optionValue = new ProductOptionValue(
            this,
            label,
            value,
            priceAdjustment,
            colorCode,
            displayOrder);

        _values.Add(optionValue);
    }


    public void DeactivateValue(int valueId)
    {
        var value = _values.FirstOrDefault(
            x => x.Id == valueId);

        if (value is null)
        {
            throw new InvalidOperationException(
                "Product option value was not found.");
        }

        value.Deactivate();
    }


    public void ActivateValue(int valueId)
    {
        var value = _values.FirstOrDefault(
            x => x.Id == valueId);

        if (value is null)
        {
            throw new InvalidOperationException(
                "Product option value was not found.");
        }

        value.Activate();
    }


    public void UpdateValue(
        int valueId,
        string label,
        string value,
        decimal priceAdjustment,
        string? colorCode,
        int displayOrder)
    {
        var optionValue = _values.FirstOrDefault(
            x => x.Id == valueId);

        if (optionValue is null)
        {
            throw new InvalidOperationException(
                "Product option value was not found.");
        }

        optionValue.Update(
            label,
            value,
            priceAdjustment,
            colorCode,
            displayOrder);
    }


    public void Update(
        bool isRequired,
        int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        IsRequired = isRequired;
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