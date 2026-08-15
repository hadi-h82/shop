using Sevart.Domain.Common;
using Sevart.Domain.Enums;

namespace Sevart.Domain.Entities;

public class Product : BaseAuditableEntity
{

    private Product()
    {
    }

    // برای ساخت Product جدید
    public Product(
        int categoryId,
        string name,
        string slug,
        string? description,
        decimal price,
        int displayOrder)
    {
        if (categoryId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(categoryId),
                "Category id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name cannot be empty.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Product slug cannot be empty.",
                nameof(slug));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Product price cannot be negative.");
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }


        CategoryId = categoryId;
        Name = name;
        Slug = slug;
        Description = description;
        Price = price;
        DisplayOrder = displayOrder;

        Status = ProductStatus.Draft;
    }

    public int CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public ProductStatus Status { get; private set; } = ProductStatus.Draft;

    public int DisplayOrder { get; private set; }


    public void Publish()
    {
        Status = ProductStatus.Published;
    }

    public void Archive()
    {
        Status = ProductStatus.Archived;
    }

    public void Update(
    int categoryId,
    string name,
    string slug,
    string? description,
    decimal price,
    int displayOrder)
    {

        if (categoryId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(categoryId),
                "Category id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name cannot be empty.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Product slug cannot be empty.",
                nameof(slug));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Product price cannot be negative.");
        }

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        CategoryId = categoryId;
        Name = name;
        Slug = slug;
        Description = description;
        Price = price;
        DisplayOrder = displayOrder;
    }


}