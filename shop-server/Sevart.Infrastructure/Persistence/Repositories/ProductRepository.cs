using Microsoft.EntityFrameworkCore;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;
using Sevart.Domain.Enums;

namespace Sevart.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SevartDbContext _dbContext;

    public ProductRepository(SevartDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetPublishedByCategorySlugAsync(
        string categorySlug,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(x => x.Images)
            .Where(x =>
                x.Category.Slug == categorySlug &&
                x.Category.IsActive &&
                x.Status == ProductStatus.Published)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}