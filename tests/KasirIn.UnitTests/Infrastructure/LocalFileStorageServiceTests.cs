namespace KasirIn.UnitTests.Infrastructure;

using KasirIn.Infrastructure.Storage;
using Xunit;

public class LocalFileStorageServiceTests
{
    [Fact]
    public async Task UploadImageAsync_ValidStream_SavesFileAndReturnsUrl()
    {
        // Arrange
        var service = new LocalFileStorageService();
        using var memoryStream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var fileName = "test-product.png";

        // Act
        var resultUrl = await service.UploadImageAsync(memoryStream, fileName);

        // Assert
        Assert.NotNull(resultUrl);
        Assert.StartsWith("/uploads/products/", resultUrl);
        Assert.EndsWith(".png", resultUrl);

        // Cleanup
        await service.DeleteImageAsync(resultUrl);
    }

    [Fact]
    public async Task UploadImageAsync_EmptyStream_ThrowsArgumentException()
    {
        // Arrange
        var service = new LocalFileStorageService();
        using var emptyStream = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.UploadImageAsync(emptyStream, "test.png"));
    }

    [Fact]
    public async Task DeleteImageAsync_ExistingFile_DeletesFile()
    {
        // Arrange
        var service = new LocalFileStorageService();
        using var memoryStream = new MemoryStream(new byte[] { 10, 20, 30 });
        var url = await service.UploadImageAsync(memoryStream, "delete-me.jpg");

        // Act
        var deleted = await service.DeleteImageAsync(url);

        // Assert
        Assert.True(deleted);
    }
}
