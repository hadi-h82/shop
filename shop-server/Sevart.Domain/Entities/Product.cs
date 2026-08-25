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

    private readonly List<ProductImage> _images = new();

    public IReadOnlyCollection<ProductImage> Images
        => _images.AsReadOnly();

    private readonly List<ProductOption> _options = new();

    public IReadOnlyCollection<ProductOption> Options
        => _options.AsReadOnly();

    public void AddImage(
        string url,
        int displayOrder,
        bool isPrimary = false)
    {
        var image = new ProductImage(
            this,
            url,
            displayOrder);

        if (isPrimary || _images.Count == 0)
        {
            foreach (var productImage in _images)
            {
                productImage.RemoveAsPrimary();
            }

            image.SetAsPrimary();
        }

        _images.Add(image);
    }

    public void SetPrimaryImage(int imageId)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
        {
            throw new InvalidOperationException(
                "Product image was not found.");
        }

        foreach (var productImage in _images)
        {
            productImage.RemoveAsPrimary();
        }

        image.SetAsPrimary();
    }

    public void RemoveImage(int imageId)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
        {
            throw new InvalidOperationException(
                "Product image was not found.");
        }

        var wasPrimary = image.IsPrimary;

        _images.Remove(image);

        if (wasPrimary && _images.Count > 0)
        {
            var newPrimaryImage = _images
                .OrderBy(x => x.DisplayOrder)
                .First();

            newPrimaryImage.SetAsPrimary();
        }
    }

    public ProductOption AddOption(
    ProductOptionDefinition definition,
    bool isRequired,
    int displayOrder)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(
                nameof(definition));
        }

        var alreadyExists = _options.Any(
            x => x.ProductOptionDefinitionId == definition.Id);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "This product option definition has already been added to the product.");
        }

        var option = new ProductOption(
            this,
            definition,
            isRequired,
            displayOrder);

        _options.Add(option);

        return option;
    }


    public void AddOptionValue(
    int optionId,
    string label,
    string value,
    decimal priceAdjustment,
    string? colorCode,
    int displayOrder)
    {
        var option = _options.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.AddValue(
            label,
            value,
            priceAdjustment,
            colorCode,
            displayOrder);
    }


    public void DeactivateOption(int optionId)
    {
        var option = _options.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.Deactivate();
    }

    public void ActivateOption(int optionId)
    {
        var option = _options.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.Activate();
    }


    public void UpdateOption(
    int optionId,
    bool isRequired,
    int displayOrder)
    {
        var option = _options.FirstOrDefault(
            x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.Update(
            isRequired,
            displayOrder);
    }

    public void DeactivateOptionValue(
    int optionId,
    int valueId)
    {
        var option = _options.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.DeactivateValue(valueId);
    }


    public void ActivateOptionValue(
    int optionId,
    int valueId)
    {
        var option = _options.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.ActivateValue(valueId);
    }

    public void UpdateOptionValue(
    int optionId,
    int valueId,
    string label,
    string value,
    decimal priceAdjustment,
    string? colorCode,
    int displayOrder)
    {
        var option = _options.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            throw new InvalidOperationException(
                "Product option was not found.");
        }

        option.UpdateValue(
            valueId,
            label,
            value,
            priceAdjustment,
            colorCode,
            displayOrder);
    }

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