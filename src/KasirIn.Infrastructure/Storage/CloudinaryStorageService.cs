namespace KasirIn.Infrastructure.Storage;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KasirIn.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

public class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration configuration)
    {
        var cloudinaryUrl = configuration["Cloudinary:Url"] ?? configuration["CLOUDINARY_URL"];

        if (!string.IsNullOrWhiteSpace(cloudinaryUrl))
        {
            _cloudinary = new Cloudinary(cloudinaryUrl);
        }
        else
        {
            var cloudName = configuration["Cloudinary:CloudName"] ?? string.Empty;
            var apiKey = configuration["Cloudinary:ApiKey"] ?? string.Empty;
            var apiSecret = configuration["Cloudinary:ApiSecret"] ?? string.Empty;

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream cannot be empty", nameof(fileStream));
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "products",
            Format = "webp"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString() ?? string.Empty;
    }

    public async Task<bool> DeleteImageAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return false;
        }

        var publicId = ExtractPublicIdFromUrl(fileUrl);
        if (string.IsNullOrEmpty(publicId))
        {
            return false;
        }

        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        return result.Result == "ok" || result.Result == "not_found";
    }

    private static string ExtractPublicIdFromUrl(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var path = uri.AbsolutePath;
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var uploadIndex = Array.IndexOf(parts, "upload");
            if (uploadIndex == -1 || uploadIndex >= parts.Length - 1)
            {
                return string.Empty;
            }

            var startIndex = uploadIndex + 1;
            if (startIndex < parts.Length && parts[startIndex].StartsWith("v") && long.TryParse(parts[startIndex][1..], out _))
            {
                startIndex++;
            }

            var pathWithoutVersion = string.Join("/", parts.Skip(startIndex));
            var dotIndex = pathWithoutVersion.LastIndexOf('.');
            return dotIndex > 0 ? pathWithoutVersion[..dotIndex] : pathWithoutVersion;
        }
        catch
        {
            return string.Empty;
        }
    }
}
