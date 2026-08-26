using Microsoft.EntityFrameworkCore;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;
using Sevart.Domain.Enums;
using Sevart.Infrastructure.Persistence;

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

            .Include(x => x.Options)
                .ThenInclude(x => x.ProductOptionDefinition)

            .Include(x => x.Options)
                .ThenInclude(x => x.Values)

            .Where(x =>
                x.Category.Slug == categorySlug &&
                x.Category.IsActive &&
                x.Status == ProductStatus.Published)

            .OrderBy(x => x.DisplayOrder)

            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(
            product,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }


    public async Task<Product?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()

            .Include(x => x.Images)

            .Include(x => x.Options)
                .ThenInclude(x => x.ProductOptionDefinition)

            .Include(x => x.Options)
                .ThenInclude(x => x.Values)

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }



    public async Task<IReadOnlyList<Product>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()

            .Include(x => x.Category)

            .Include(x => x.Images)

            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.Id)

            .ToListAsync(cancellationToken);
    }



    public async Task UpdateAsync(
    Product product,
    CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }


    public async Task DeleteAsync(
    Product product,
    CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(product);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}