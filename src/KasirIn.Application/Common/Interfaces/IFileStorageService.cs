namespace KasirIn.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteImageAsync(string fileUrl, CancellationToken cancellationToken = default);
}
