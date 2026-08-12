namespace KasirIn.Application.Products.Commands.CreateProduct;

using KasirIn.Application.Common.Interfaces;
using KasirIn.Domain.Entities;
using MediatR;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IKasirInDbContext _context;
    private readonly IFileStorageService? _fileStorageService;

    public CreateProductCommandHandler(IKasirInDbContext context, IFileStorageService? fileStorageService = null)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = request.ImageUrl;

        if (request.ImageStream != null && !string.IsNullOrEmpty(request.ImageFileName) && _fileStorageService != null)
        {
            imageUrl = await _fileStorageService.UploadImageAsync(request.ImageStream, request.ImageFileName, cancellationToken);
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            SKU = request.SKU,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            StockQuantity = request.StockQuantity,
            MinStockThreshold = request.MinStockThreshold,
            ImageUrl = imageUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
