using Sevart.Application.Abstractions.Storage;

namespace Sevart.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;
    private readonly string _baseUrl;

    public LocalFileStorage(
        string rootPath,
        string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException(
                "Storage root path is required.",
                nameof(rootPath));
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException(
                "Storage base URL is required.",
                nameof(baseUrl));
        }

        _rootPath =
            Path.GetFullPath(rootPath);

        _baseUrl =
            $"/{baseUrl.Trim('/')}";
    }

    public async Task<string> SaveAsync(
        Stream stream,
        string directoryName,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException(
                "The file stream must be readable.",
                nameof(stream));
        }

        ValidateDirectoryName(
            directoryName);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));
        }

        var extension =
            Path.GetExtension(fileName)
                .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException(
                "File extension is required.",
                nameof(fileName));
        }

        var now =
            DateTime.UtcNow;

        var relativeDirectory =
            Path.Combine(
                directoryName,
                now.Year.ToString(),
                now.Month.ToString("00"));

        var physicalDirectory =
            Path.Combine(
                _rootPath,
                relativeDirectory);

        Directory.CreateDirectory(
            physicalDirectory);

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var physicalFilePath =
            Path.Combine(
                physicalDirectory,
                storedFileName);

        try
        {
            await using var destinationStream =
                new FileStream(
                    physicalFilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

            await stream.CopyToAsync(
                destinationStream,
                cancellationToken);
        }
        catch
        {
            if (File.Exists(physicalFilePath))
            {
                File.Delete(physicalFilePath);
            }

            throw;
        }

        var relativeUrl =
            Path.Combine(
                    relativeDirectory,
                    storedFileName)
                .Replace(
                    Path.DirectorySeparatorChar,
                    '/');

        return $"{_baseUrl}/{relativeUrl}";
    }

    public Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        var urlPath =
            GetUrlPath(fileUrl);

        if (!urlPath.StartsWith(
                $"{_baseUrl}/",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The file URL does not belong to this storage.");
        }

        var relativePath =
            urlPath[_baseUrl.Length..]
                .TrimStart('/');

        relativePath =
            Uri.UnescapeDataString(
                relativePath);

        var physicalFilePath =
            Path.GetFullPath(
                Path.Combine(
                    _rootPath,
                    relativePath.Replace(
                        '/',
                        Path.DirectorySeparatorChar)));

        var rootPathWithSeparator =
            _rootPath.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!physicalFilePath.StartsWith(
                rootPathWithSeparator,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The resolved file path is outside the storage root.");
        }

        if (File.Exists(physicalFilePath))
        {
            File.Delete(physicalFilePath);
        }

        return Task.CompletedTask;
    }

    private static void ValidateDirectoryName(
        string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
        {
            throw new ArgumentException(
                "Directory name is required.",
                nameof(directoryName));
        }

        var isValid =
            directoryName.All(
                character =>
                    char.IsLetterOrDigit(character) ||
                    character == '-' ||
                    character == '_');

        if (!isValid)
        {
            throw new ArgumentException(
                "Directory name contains invalid characters.",
                nameof(directoryName));
        }
    }

    private static string GetUrlPath(
        string fileUrl)
    {
        if (Uri.TryCreate(
                fileUrl,
                UriKind.Absolute,
                out var absoluteUri))
        {
            return absoluteUri.AbsolutePath;
        }

        var queryIndex =
            fileUrl.IndexOfAny(
                ['?', '#']);

        return queryIndex >= 0
            ? fileUrl[..queryIndex]
            : fileUrl;
    }
}