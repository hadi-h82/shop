using Sevart.Domain.Entities;

namespace Sevart.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetPublishedByCategorySlugAsync(
        string categorySlug,
        CancellationToken cancellationToken = default);
}