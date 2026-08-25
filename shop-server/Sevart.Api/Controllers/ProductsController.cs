using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.Products;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductOptionDefinitionRepository _optionDefinitionRepository;

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IProductOptionDefinitionRepository optionDefinitionRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _optionDefinitionRepository = optionDefinitionRepository;
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

    private static ProductResponse ToResponse(Product product)
    {
        var primaryImage = product.Images
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.DisplayOrder)
            .FirstOrDefault();

        return new ProductResponse
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = primaryImage?.Url,
            DisplayOrder = product.DisplayOrder,

            Options = product.Options
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(option => new ProductOptionResponse
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

                    Values = option.Values
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.DisplayOrder)
                        .Select(value => new ProductOptionValueResponse
                        {
                            Id = value.Id,
                            Label = value.Label,
                            Value = value.Value,
                            PriceAdjustment = value.PriceAdjustment,
                            ColorCode = value.ColorCode,
                            IsActive = value.IsActive,
                            DisplayOrder = value.DisplayOrder
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
}