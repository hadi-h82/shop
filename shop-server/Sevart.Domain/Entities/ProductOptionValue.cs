using Sevart.Domain.Common;

namespace Sevart.Domain.Entities;

public class ProductOptionValue : BaseEntity
{

    // برای Entity Framework
    private ProductOptionValue()
    {
    }

    // برای ساخت ProductOptionValue جدید
    public ProductOptionValue(
        ProductOption productOption,
        string label,
        string value,
        decimal priceAdjustment,
        string? colorCode,
        int displayOrder)
    {
        if (productOption is null)
        {
            throw new ArgumentNullException(nameof(productOption));
        }

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

        ProductOption = productOption;
        Label = label;
        Value = value;
        PriceAdjustment = priceAdjustment;
        ColorCode = colorCode;
        DisplayOrder = displayOrder;

        IsActive = true;
    }

    public int ProductOptionId { get; private set; }

    public ProductOption ProductOption { get; private set; } = null!;
    public string Label { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public decimal PriceAdjustment { get; private set; }

    public string? ColorCode { get; private set; }

    public bool IsActive { get; private set; } = true;

    public int DisplayOrder { get; private set; }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(
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

        Label = label;
        Value = value;
        PriceAdjustment = priceAdjustment;
        ColorCode = colorCode;
        DisplayOrder = displayOrder;
    }




}