using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.ProductOptionDefinitions;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductOptionDefinitionsController : ControllerBase
{
    private readonly IProductOptionDefinitionRepository _repository;

    public ProductOptionDefinitionsController(
        IProductOptionDefinitionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var definitions = await _repository.GetAllAsync(
            cancellationToken);

        var response = definitions
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var definition = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (definition is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(definition));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductOptionDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var slugExists = await _repository.SlugExistsAsync(
            request.Slug,
            cancellationToken: cancellationToken);

        if (slugExists)
        {
            return Conflict(new
            {
                message = "A product option definition with this slug already exists."
            });
        }

        var definition = new ProductOptionDefinition(
            request.Name,
            request.Slug,
            request.InputType,
            request.DisplayOrder);

        await _repository.AddAsync(
            definition,
            cancellationToken);

        return Ok(ToResponse(definition));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductOptionDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var definition = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (definition is null)
        {
            return NotFound();
        }

        var slugExists = await _repository.SlugExistsAsync(
            request.Slug,
            id,
            cancellationToken);

        if (slugExists)
        {
            return Conflict(new
            {
                message = "A product option definition with this slug already exists."
            });
        }

        definition.Update(
            request.Name,
            request.Slug,
            request.InputType,
            request.DisplayOrder);

        await _repository.UpdateAsync(
            definition,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(
        int id,
        CancellationToken cancellationToken)
    {
        var definition = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (definition is null)
        {
            return NotFound();
        }

        definition.Activate();

        await _repository.UpdateAsync(
            definition,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        var definition = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (definition is null)
        {
            return NotFound();
        }

        definition.Deactivate();

        await _repository.UpdateAsync(
            definition,
            cancellationToken);

        return NoContent();
    }

    private static ProductOptionDefinitionResponse ToResponse(
        ProductOptionDefinition definition)
    {
        return new ProductOptionDefinitionResponse
        {
            Id = definition.Id,
            Name = definition.Name,
            Slug = definition.Slug,
            InputType = definition.InputType,
            DisplayOrder = definition.DisplayOrder,
            IsActive = definition.IsActive
        };
    }
}