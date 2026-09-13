using Microsoft.EntityFrameworkCore;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;

namespace Sevart.Infrastructure.Persistence.Repositories;

public class ProductOptionDefinitionRepository
    : IProductOptionDefinitionRepository
{
    private readonly SevartDbContext _dbContext;

    public ProductOptionDefinitionRepository(
        SevartDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductOptionDefinition>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductOptionDefinitions
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductOptionDefinition?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductOptionDefinitions
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<ProductOptionDefinition?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductOptionDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Slug == slug,
                cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductOptionDefinitions
            .AnyAsync(
                x =>
                    x.Slug == slug &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    public async Task AddAsync(
        ProductOptionDefinition definition,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ProductOptionDefinitions.AddAsync(
            definition,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        ProductOptionDefinition definition,
        CancellationToken cancellationToken = default)
    {
        _dbContext.ProductOptionDefinitions.Update(definition);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}