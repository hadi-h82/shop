using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.Categories;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
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
            return NotFound();
        }

        var hasProducts = await _categoryRepository.HasProductsAsync(
            id,
            cancellationToken);

        if (hasProducts)
        {
            category.Deactivate();

            await _categoryRepository.UpdateAsync(
                category,
                cancellationToken);

            return NoContent();
        }

        await _categoryRepository.DeleteAsync(
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


}