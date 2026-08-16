using Sevart.Domain.Common;
using Sevart.Domain.Enums;

namespace Sevart.Domain.Entities;

public class ProductOption : BaseEntity
{

    // برای Entity Framework
    private ProductOption()
    {
    }

    // برای ساخت ProductOption جدید
    public ProductOption(
        Product product,
        string name,
        ProductOptionInputType inputType,
        bool isRequired,
        int displayOrder)
    {

        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product option name cannot be empty.",
                nameof(name));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Product = product;
        Name = name;
        InputType = inputType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public int ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;

    public ProductOptionInputType InputType { get; private set; }

    public bool IsRequired { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<ProductOptionValue> _values = new();

    public IReadOnlyCollection<ProductOptionValue> Values
        => _values.AsReadOnly();


    public void AddValue(
    string label,
    string value,
    decimal priceAdjustment,
    string? colorCode,
    int displayOrder)
    {
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
        var value = _values.FirstOrDefault(x => x.Id == valueId);

        if (value is null)
        {
            throw new InvalidOperationException(
                "Product option value was not found.");
        }

        value.Deactivate();
    }


    public void ActivateValue(int valueId)
    {
        var value = _values.FirstOrDefault(x => x.Id == valueId);

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
        var optionValue = _values.FirstOrDefault(x => x.Id == valueId);

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
    string name,
    ProductOptionInputType inputType,
    bool isRequired,
    int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product option name cannot be empty.",
                nameof(name));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = name;
        InputType = inputType;
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