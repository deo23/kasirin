namespace KasirIn.Application.Products.Commands.CreateProduct;

using MediatR;

public record CreateProductCommand : IRequest<Guid>
{
    public Guid TenantId { get; init; }
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal CostPrice { get; init; }
    public decimal SellingPrice { get; init; }
    public int StockQuantity { get; init; }
    public int MinStockThreshold { get; init; }
    public Stream? ImageStream { get; init; }
    public string? ImageFileName { get; init; }
    public string? ImageUrl { get; init; }
}
