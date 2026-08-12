namespace KasirIn.Application.Products.Queries.GetProducts;

public record ProductDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal CostPrice { get; init; }
    public decimal SellingPrice { get; init; }
    public int StockQuantity { get; init; }
    public int MinStockThreshold { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsLowStock { get; init; }
}
