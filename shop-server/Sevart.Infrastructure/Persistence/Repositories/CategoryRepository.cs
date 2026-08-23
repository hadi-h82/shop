using Microsoft.EntityFrameworkCore;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Domain.Entities;

namespace Sevart.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SevartDbContext _dbContext;

    public CategoryRepository(SevartDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(
            category,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }


    public async Task UpdateAsync(
    Category category,
    CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Update(category);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }


    public async Task<bool> HasProductsAsync(
    int categoryId,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AnyAsync(
                x => x.CategoryId == categoryId,
                cancellationToken);
    }



    public async Task DeleteAsync(
    Category category,
    CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Remove(category);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Category?> GetBySlugAsync(
    string slug,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Slug == slug && x.IsActive,
                cancellationToken);
    }
}