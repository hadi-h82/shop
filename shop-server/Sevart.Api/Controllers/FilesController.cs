using Microsoft.AspNetCore.Mvc;
using Sevart.Application.Abstractions.Storage;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private const long MaximumFileSize =
        5 * 1024 * 1024;

    private readonly IFileStorage _fileStorage;

    public FilesController(
        IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost("product-images")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public Task<IActionResult> UploadProductImage(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        return UploadImage(
            file,
            "products",
            cancellationToken);
    }

    [HttpPost("category-images")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public Task<IActionResult> UploadCategoryImage(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        return UploadImage(
            file,
            "categories",
            cancellationToken);
    }

    private async Task<IActionResult> UploadImage(
        IFormFile? file,
        string directoryName,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message =
                    "لطفاً یک تصویر انتخاب کنید."
            });
        }

        if (file.Length > MaximumFileSize)
        {
            return BadRequest(new
            {
                message =
                    "حجم تصویر نباید بیشتر از ۵ مگابایت باشد."
            });
        }

        await using var stream =
            file.OpenReadStream();

        var detectedImage =
            await DetectImageAsync(
                stream,
                cancellationToken);

        if (detectedImage is null)
        {
            return BadRequest(new
            {
                message =
                    "فقط تصاویر JPG، PNG و WebP مجاز هستند."
            });
        }

        stream.Position = 0;

        var storageFileName =
            $"{Path.GetFileNameWithoutExtension(file.FileName)}" +
            $"{detectedImage.Extension}";

        var fileUrl =
            await _fileStorage.SaveAsync(
                stream,
                directoryName,
                storageFileName,
                detectedImage.ContentType,
                cancellationToken);

        var absoluteUrl =
            $"{Request.Scheme}://{Request.Host}{fileUrl}";

        return Ok(new
        {
            url = fileUrl,
            absoluteUrl,
            fileName =
                Path.GetFileName(fileUrl),
            contentType =
                detectedImage.ContentType,
            size = file.Length
        });
    }

    private static async Task<DetectedImage?> DetectImageAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var header = new byte[12];
        var bytesRead = 0;

        while (bytesRead < header.Length)
        {
            var read =
                await stream.ReadAsync(
                    header.AsMemory(
                        bytesRead,
                        header.Length - bytesRead),
                    cancellationToken);

            if (read == 0)
            {
                break;
            }

            bytesRead += read;
        }

        if (bytesRead >= 3 &&
            header[0] == 0xFF &&
            header[1] == 0xD8 &&
            header[2] == 0xFF)
        {
            return new DetectedImage(
                ".jpg",
                "image/jpeg");
        }

        if (bytesRead >= 8 &&
            header[0] == 0x89 &&
            header[1] == 0x50 &&
            header[2] == 0x4E &&
            header[3] == 0x47 &&
            header[4] == 0x0D &&
            header[5] == 0x0A &&
            header[6] == 0x1A &&
            header[7] == 0x0A)
        {
            return new DetectedImage(
                ".png",
                "image/png");
        }

        if (bytesRead >= 12 &&
            header[0] == 0x52 &&
            header[1] == 0x49 &&
            header[2] == 0x46 &&
            header[3] == 0x46 &&
            header[8] == 0x57 &&
            header[9] == 0x45 &&
            header[10] == 0x42 &&
            header[11] == 0x50)
        {
            return new DetectedImage(
                ".webp",
                "image/webp");
        }

        return null;
    }

    private sealed record DetectedImage(
        string Extension,
        string ContentType);
}