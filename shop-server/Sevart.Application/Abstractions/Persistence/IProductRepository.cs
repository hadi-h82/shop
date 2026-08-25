using Sevart.Domain.Entities;

namespace Sevart.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetPublishedByCategorySlugAsync(
        string categorySlug,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(
    CancellationToken cancellationToken = default);
}