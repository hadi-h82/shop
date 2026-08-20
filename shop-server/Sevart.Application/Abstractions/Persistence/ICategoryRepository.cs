using Sevart.Domain.Entities;

namespace Sevart.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
    Category category,
    CancellationToken cancellationToken = default);

    Task<bool> HasProductsAsync(
    int categoryId,
    CancellationToken cancellationToken = default);

    Task DeleteAsync(
    Category category,
    CancellationToken cancellationToken = default);
}