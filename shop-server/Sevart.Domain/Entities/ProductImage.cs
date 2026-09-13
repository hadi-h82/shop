using Sevart.Domain.Common;

namespace Sevart.Domain.Entities;

public class ProductImage : BaseEntity
{

    // برای Entity Framework
    private ProductImage()
    {
    }

    // برای ساخت ProductImage جدید
    public ProductImage(
        Product product,
        string url,
        int displayOrder)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException(
                "Product image URL cannot be empty.",
                nameof(url));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Product = product;
        Url = url;
        DisplayOrder = displayOrder;

        IsPrimary = false;
    }
    public int ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public string Url { get; private set; } = string.Empty;

    public bool IsPrimary { get; private set; }

    public int DisplayOrder { get; private set; }

    internal void SetAsPrimary()
    {
        IsPrimary = true;
    }

    internal void RemoveAsPrimary()
    {
        IsPrimary = false;
    }

    public void Update(
    string url,
    int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException(
                "Product image URL cannot be empty.",
                nameof(url));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Url = url;
        DisplayOrder = displayOrder;
    }
}