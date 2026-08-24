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

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
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
            DisplayOrder = product.DisplayOrder
        };
    }
}