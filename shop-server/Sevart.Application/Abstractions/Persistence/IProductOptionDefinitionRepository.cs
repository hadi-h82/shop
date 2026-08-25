using Sevart.Domain.Entities;

namespace Sevart.Application.Abstractions.Persistence;

public interface IProductOptionDefinitionRepository
{
    Task<IReadOnlyList<ProductOptionDefinition>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductOptionDefinition?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductOptionDefinition?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(
        string slug,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductOptionDefinition definition,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ProductOptionDefinition definition,
        CancellationToken cancellationToken = default);
}