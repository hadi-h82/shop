using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.Categories;
using Sevart.Api.Contracts.Products;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CategoriesController(
        ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(
            cancellationToken);

        var response = categories
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(category));
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(
    string slug,
    CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetBySlugAsync(
            slug,
            cancellationToken);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(category));
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
    int id,
    UpdateCategoryRequest request,
    CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return NotFound();
        }

        category.Update(
            request.Name,
            request.Slug,
            request.Description,
            request.ImageUrl,
            request.DisplayOrder);

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
     int id,
     CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return NotFound(new
            {
                message = "دسته‌بندی پیدا نشد."
            });
        }

        var hasProducts =
            await _categoryRepository.HasProductsAsync(
                id,
                cancellationToken);

        if (hasProducts)
        {
            return Conflict(new
            {
                message =
                    "این دسته‌بندی دارای محصول می‌باشد و امکان حذف آن وجود ندارد."
            });
        }

        await _categoryRepository.DeleteAsync(
            category,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(
    int id,
    CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return NotFound(new
            {
                message = "دسته‌بندی پیدا نشد."
            });
        }

        category.Activate();

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        return NoContent();
    }


    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(
    int id,
    CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return NotFound(new
            {
                message = "دسته‌بندی پیدا نشد."
            });
        }

        category.Deactivate();

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    CreateCategoryRequest request,
    CancellationToken cancellationToken)
    {
        var category = new Category(
            request.Name,
            request.Slug,
            request.Description,
            request.ImageUrl,
            request.DisplayOrder);

        await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        return Ok(category.Id);
    }


    private static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        };
    }

    private static ProductResponse ToProductResponse(Product product)
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



    [HttpGet("{slug}/products")]
    public async Task<IActionResult> GetWithProducts(
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

        var response = new CategoryDetailsResponse
        {
            Category = ToResponse(category),

            Products = products
                .Select(ToProductResponse)
                .ToList()
        };

        return Ok(response);
    }


}