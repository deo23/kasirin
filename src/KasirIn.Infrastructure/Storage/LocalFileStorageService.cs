namespace KasirIn.Infrastructure.Storage;

using KasirIn.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadFolder;
    private const string RelativeUrlPath = "/uploads/products";

    public LocalFileStorageService(IWebHostEnvironment? webHostEnvironment = null)
    {
        var webRoot = !string.IsNullOrEmpty(webHostEnvironment?.WebRootPath)
            ? webHostEnvironment.WebRootPath
            : Path.Combine(AppContext.BaseDirectory, "wwwroot");

        _uploadFolder = Path.Combine(webRoot, "uploads", "products");
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream cannot be empty", nameof(fileStream));
        }

        if (!Directory.Exists(_uploadFolder))
        {
            Directory.CreateDirectory(_uploadFolder);
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension))
        {
            extension = ".jpg";
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream, cancellationToken);
        }

        return $"{RelativeUrlPath}/{uniqueFileName}";
    }

    public Task<bool> DeleteImageAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.FromResult(false);
        }

        try
        {
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadFolder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
