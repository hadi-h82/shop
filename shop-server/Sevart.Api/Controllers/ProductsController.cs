using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.Products;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;
using Sevart.Application.Abstractions.Storage;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductOptionDefinitionRepository _optionDefinitionRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IProductOptionDefinitionRepository optionDefinitionRepository,
        IFileStorage fileStorage,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _optionDefinitionRepository = optionDefinitionRepository;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    [HttpGet("category/{slug}")]
    public async Task<IActionResult> GetByCategory(
        string slug,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetBySlugAsync(
            slug,
            cancellationToken);

        if (category is null || !category.IsActive)
        {
            return NotFound();
        }

        var products =
            await _productRepository.GetPublishedByCategorySlugAsync(
                slug,
                cancellationToken);

        var response = products
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null || !category.IsActive)
        {
            return BadRequest(new
            {
                message = "Category was not found or is inactive."
            });
        }

        var product = new Product(
            request.CategoryId,
            request.Name,
            request.Slug,
            request.Description,
            request.Price,
            request.DisplayOrder);

        foreach (var optionRequest in request.Options)
        {
            var definition =
                await _optionDefinitionRepository.GetByIdAsync(
                    optionRequest.ProductOptionDefinitionId,
                    cancellationToken);

            if (definition is null || !definition.IsActive)
            {
                return BadRequest(new
                {
                    message =
                        $"Product option definition with id {optionRequest.ProductOptionDefinitionId} was not found or is inactive."
                });
            }

            var option = product.AddOption(
                definition,
                optionRequest.IsRequired,
                optionRequest.DisplayOrder);

            foreach (var valueRequest in optionRequest.Values)
            {
                option.AddValue(
                    valueRequest.Label,
                    valueRequest.Value,
                    valueRequest.PriceAdjustment,
                    valueRequest.ColorCode,
                    valueRequest.DisplayOrder);
            }
        }

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        return Ok(new
        {
            id = product.Id
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
    int id,
    CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(ToAdminResponse(product));
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(
    int id,
    CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        product.Publish();

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        return NoContent();
    }


    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        product.Archive();

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        return NoContent();
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
     int id,
     UpdateProductRequest request,
     CancellationToken cancellationToken)
    {
        // =========================
        // Load Product
        // =========================

        var product = await _productRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Product was not found."
            });
        }

        // =========================
        // Validate Category
        // =========================

        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null || !category.IsActive)
        {
            return BadRequest(new
            {
                message = "Category was not found or is inactive."
            });
        }

        // =========================
        // Validate Option Definitions
        // =========================

        var definitions =
            new Dictionary<int, ProductOptionDefinition>();

        foreach (
            var definitionId in request.Options
                .Select(x => x.ProductOptionDefinitionId)
                .Distinct()
        )
        {
            var definition =
                await _optionDefinitionRepository.GetByIdAsync(
                    definitionId,
                    cancellationToken);

            if (definition is null || !definition.IsActive)
            {
                return BadRequest(new
                {
                    message =
                        $"Product option definition with id {definitionId} was not found or is inactive."
                });
            }

            definitions.Add(
                definitionId,
                definition);
        }

        // =========================
        // Deactivate Removed Options
        // =========================

        var requestedExistingOptionIds = request.Options
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToHashSet();

        foreach (var existingOption in product.Options)
        {
            if (
                !requestedExistingOptionIds.Contains(
                    existingOption.Id)
            )
            {
                product.DeactivateOption(
                    existingOption.Id);
            }
        }

        // =========================
        // Add / Update Options
        // =========================

        foreach (var optionRequest in request.Options)
        {
            // =========================
            // Existing Option
            // =========================

            if (optionRequest.Id.HasValue)
            {
                var existingOption = product.Options
                    .FirstOrDefault(
                        x => x.Id == optionRequest.Id.Value);

                if (existingOption is null)
                {
                    return BadRequest(new
                    {
                        message =
                            $"Product option with id {optionRequest.Id.Value} does not belong to this product."
                    });
                }

                // Definition یک Option موجود نباید عوض شود.
                if (
                    existingOption.ProductOptionDefinitionId !=
                    optionRequest.ProductOptionDefinitionId
                )
                {
                    return BadRequest(new
                    {
                        message =
                            "Product option definition cannot be changed for an existing product option."
                    });
                }

                product.UpdateOption(
                    existingOption.Id,
                    optionRequest.IsRequired,
                    optionRequest.DisplayOrder);

                product.ActivateOption(
                    existingOption.Id);

                // =========================
                // Deactivate Removed Values
                // =========================

                var requestedExistingValueIds =
                    optionRequest.Values
                        .Where(x => x.Id.HasValue)
                        .Select(x => x.Id!.Value)
                        .ToHashSet();

                foreach (
                    var existingValue in existingOption.Values
                )
                {
                    if (
                        !requestedExistingValueIds.Contains(
                            existingValue.Id)
                    )
                    {
                        product.DeactivateOptionValue(
                            existingOption.Id,
                            existingValue.Id);
                    }
                }

                // =========================
                // Add / Update Values
                // =========================

                foreach (
                    var valueRequest in optionRequest.Values
                )
                {
                    // Existing Value
                    if (valueRequest.Id.HasValue)
                    {
                        var existingValue =
                            existingOption.Values
                                .FirstOrDefault(
                                    x =>
                                        x.Id ==
                                        valueRequest.Id.Value);

                        if (existingValue is null)
                        {
                            return BadRequest(new
                            {
                                message =
                                    $"Product option value with id {valueRequest.Id.Value} does not belong to product option {existingOption.Id}."
                            });
                        }

                        product.UpdateOptionValue(
                            existingOption.Id,
                            existingValue.Id,
                            valueRequest.Label,
                            valueRequest.Value,
                            valueRequest.PriceAdjustment,
                            valueRequest.ColorCode,
                            valueRequest.DisplayOrder);

                        product.ActivateOptionValue(
                            existingOption.Id,
                            existingValue.Id);

                        continue;
                    }

                    // New Value
                    existingOption.AddValue(
                        valueRequest.Label,
                        valueRequest.Value,
                        valueRequest.PriceAdjustment,
                        valueRequest.ColorCode,
                        valueRequest.DisplayOrder);
                }

                continue;
            }

            // =========================
            // New Option
            // =========================

            var definition =
                definitions[
                    optionRequest.ProductOptionDefinitionId
                ];

            var newOption = product.AddOption(
                definition,
                optionRequest.IsRequired,
                optionRequest.DisplayOrder);

            // Valueهای Option جدید
            foreach (
                var valueRequest in optionRequest.Values
            )
            {
                newOption.AddValue(
                    valueRequest.Label,
                    valueRequest.Value,
                    valueRequest.PriceAdjustment,
                    valueRequest.ColorCode,
                    valueRequest.DisplayOrder);
            }
        }

        // =========================
        // Update Product Base Info
        // =========================

        product.Update(
            request.CategoryId,
            request.Name,
            request.Slug,
            request.Description,
            request.Price,
            request.DisplayOrder);

        // =========================
        // Save
        // =========================

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (product is null)
        {
            return NotFound(new
            {
                message =
                    "Product was not found."
            });
        }

        /*
         * آدرس تصاویر را قبل از حذف محصول نگه می‌داریم؛
         * چون پس از حذف رکورد دیگر به آن‌ها دسترسی نداریم.
         */
        var imageUrls =
            product.Images
                .Select(
                    image =>
                        image.Url)
                .Where(
                    imageUrl =>
                        !string.IsNullOrWhiteSpace(
                            imageUrl))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        /*
         * ابتدا محصول و روابط آن از دیتابیس حذف می‌شوند.
         */
        await _productRepository.DeleteAsync(
            product,
            cancellationToken);

        /*
         * سپس فایل‌های فیزیکی تصاویر پاک می‌شوند.
         * شکست در حذف یک فایل نباید پاسخ موفق
         * حذف محصول را خراب کند.
         */
        foreach (var imageUrl in imageUrls)
        {
            await TryDeleteImageAsync(
                imageUrl,
                cancellationToken);
        }

        return NoContent();
    }


    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(
    string slug,
    CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetBySlugAsync(
            slug,
            cancellationToken);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Product was not found."
            });
        }

        return Ok(ToResponse(product));
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(
            cancellationToken);

        var response = products
            .Select(product =>
            {
                var primaryImage = product.Images
                    .OrderByDescending(x => x.IsPrimary)
                    .ThenBy(x => x.DisplayOrder)
                    .FirstOrDefault();

                return new AdminProductListItemResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Slug = product.Slug,
                    Price = product.Price,
                    Status = product.Status,

                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,

                    ImageUrl = primaryImage?.Url,

                    DisplayOrder = product.DisplayOrder
                };
            })
            .ToList();

        return Ok(response);
    }


    [HttpPost("{id:int}/images")]
    public async Task<IActionResult> AddImage(
    int id,
    AddProductImageRequest request,
    CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.GetByIdForUpdateAsync(
                id,
                cancellationToken);

        if (product is null)
        {
            return NotFound(new
            {
                message = "محصول پیدا نشد."
            });
        }

        var imageUrl = request.Url.Trim();

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return BadRequest(new
            {
                message = "آدرس تصویر الزامی است."
            });
        }

        if (!IsValidImageUrl(imageUrl))
        {
            return BadRequest(new
            {
                message = "آدرس تصویر معتبر نیست."
            });
        }

        var imageAlreadyExists =
            product.Images.Any(
                image =>
                    string.Equals(
                        image.Url,
                        imageUrl,
                        StringComparison.OrdinalIgnoreCase));

        if (imageAlreadyExists)
        {
            return Conflict(new
            {
                message = "این تصویر قبلاً به محصول اضافه شده است."
            });
        }

        product.AddImage(
            imageUrl,
            request.DisplayOrder,
            request.IsPrimary);

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        return NoContent();
    }


    [HttpPatch("{id:int}/images/{imageId:int}/primary")]
    public async Task<IActionResult> SetPrimaryImage(
    int id,
    int imageId,
    CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.GetByIdForUpdateAsync(
                id,
                cancellationToken);

        if (product is null)
        {
            return NotFound(new
            {
                message = "محصول پیدا نشد."
            });
        }

        var imageExists =
            product.Images.Any(
                image => image.Id == imageId);

        if (!imageExists)
        {
            return NotFound(new
            {
                message = "تصویر محصول پیدا نشد."
            });
        }

        product.SetPrimaryImage(imageId);

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(
    int id,
    int imageId,
    CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.GetByIdForUpdateAsync(
                id,
                cancellationToken);

        if (product is null)
        {
            return NotFound(new
            {
                message = "محصول پیدا نشد."
            });
        }

        var image =
            product.Images.FirstOrDefault(
                productImage =>
                    productImage.Id == imageId);

        if (image is null)
        {
            return NotFound(new
            {
                message = "تصویر محصول پیدا نشد."
            });
        }

        var imageUrl = image.Url;

        product.RemoveImage(imageId);

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        try
        {
            await _fileStorage.DeleteAsync(
                imageUrl,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Product image file could not be deleted. Image URL: {ImageUrl}",
                imageUrl);
        }

        return NoContent();
    }

    private static bool IsValidImageUrl(string imageUrl)
    {
        if (imageUrl.StartsWith(
            "/uploads/",
            StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Uri.TryCreate(
                   imageUrl,
                   UriKind.Absolute,
                   out var absoluteUri) &&
               (absoluteUri.Scheme == Uri.UriSchemeHttps ||
                absoluteUri.Scheme == Uri.UriSchemeHttp);
    }

    private static ProductResponse ToResponse(
        Product product)
    {
        var orderedImages =
            product.Images
                .OrderByDescending(
                    image => image.IsPrimary)
                .ThenBy(
                    image => image.DisplayOrder)
                .ThenBy(
                    image => image.Id)
                .ToList();

        var primaryImage =
            orderedImages.FirstOrDefault();

        return new ProductResponse
        {
            Id =
                product.Id,

            CategoryId =
                product.CategoryId,

            CategoryName =
    product.Category.Name,

            CategorySlug =
    product.Category.Slug,

            Name =
                product.Name,

            Slug =
                product.Slug,

            Description =
                product.Description,

            Price =
                product.Price,

            /*
             * برای سازگاری با صفحه لیست محصولات،
             * تصویر اصلی همچنان جداگانه برگردانده می‌شود.
             */
            ImageUrl =
                primaryImage?.Url,

            DisplayOrder =
                product.DisplayOrder,

            /*
             * تمام تصاویر برای گالری صفحه جزئیات
             */
            Images =
                orderedImages
                    .Select(image =>
                        new ProductImageResponse
                        {
                            Id =
                                image.Id,

                            Url =
                                image.Url,

                            IsPrimary =
                                image.IsPrimary,

                            DisplayOrder =
                                image.DisplayOrder
                        })
                    .ToList(),

            Options =
                product.Options
                    .Where(
                        option =>
                            option.IsActive)
                    .OrderBy(
                        option =>
                            option.DisplayOrder)
                    .Select(option =>
                        new ProductOptionResponse
                        {
                            Id =
                                option.Id,

                            ProductOptionDefinitionId =
                                option.ProductOptionDefinitionId,

                            Name =
                                option
                                    .ProductOptionDefinition
                                    .Name,

                            InputType =
                                option
                                    .ProductOptionDefinition
                                    .InputType,

                            IsRequired =
                                option.IsRequired,

                            DisplayOrder =
                                option.DisplayOrder,

                            Values =
                                option.Values
                                    .Where(
                                        value =>
                                            value.IsActive)
                                    .OrderBy(
                                        value =>
                                            value.DisplayOrder)
                                    .Select(value =>
                                        new ProductOptionValueResponse
                                        {
                                            Id =
                                                value.Id,

                                            Label =
                                                value.Label,

                                            Value =
                                                value.Value,

                                            PriceAdjustment =
                                                value.PriceAdjustment,

                                            ColorCode =
                                                value.ColorCode,

                                            IsActive =
                                                value.IsActive,

                                            DisplayOrder =
                                                value.DisplayOrder
                                        })
                                    .ToList()
                        })
                    .ToList()
        };
    }


    private static AdminProductResponse ToAdminResponse(Product product)
    {
        return new AdminProductResponse
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            Status = product.Status,
            DisplayOrder = product.DisplayOrder,

            Images = product.Images
                .OrderBy(x => x.DisplayOrder)
                .Select(image => new AdminProductImageResponse
                {
                    Id = image.Id,
                    Url = image.Url,
                    IsPrimary = image.IsPrimary,
                    DisplayOrder = image.DisplayOrder
                })
                .ToList(),

            Options = product.Options
                .OrderBy(x => x.DisplayOrder)
                .Select(option => new AdminProductOptionResponse
                {
                    Id = option.Id,
                    ProductOptionDefinitionId =
                        option.ProductOptionDefinitionId,
                    Name =
                        option.ProductOptionDefinition.Name,
                    InputType =
                        option.ProductOptionDefinition.InputType,
                    IsRequired =
                        option.IsRequired,
                    DisplayOrder =
                        option.DisplayOrder,
                    IsActive =
                        option.IsActive,

                    Values = option.Values
                        .OrderBy(x => x.DisplayOrder)
                        .Select(value =>
                            new AdminProductOptionValueResponse
                            {
                                Id = value.Id,
                                Label = value.Label,
                                Value = value.Value,
                                PriceAdjustment =
                                    value.PriceAdjustment,
                                ColorCode =
                                    value.ColorCode,
                                IsActive =
                                    value.IsActive,
                                DisplayOrder =
                                    value.DisplayOrder
                            })
                        .ToList()
                })
                .ToList()
        };
    }


    private async Task TryDeleteImageAsync(
    string? imageUrl,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
                imageUrl))
        {
            return;
        }

        try
        {
            await _fileStorage.DeleteAsync(
                imageUrl,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Product image file could not be deleted. Image URL: {ImageUrl}",
                imageUrl);
        }
    }
}