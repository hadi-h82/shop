namespace Sevart.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> SaveAsync(
        Stream stream,
        string directoryName,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default);
}