namespace KasirIn.Application.Products.Queries.GetProducts;

using MediatR;

public record GetProductsQuery : IRequest<List<ProductDto>>
{
    public Guid TenantId { get; init; }
    public Guid? CategoryId { get; init; }
    public string? SearchTerm { get; init; }

    public GetProductsQuery(Guid tenantId, Guid? categoryId = null, string? searchTerm = null)
    {
        TenantId = tenantId;
        CategoryId = categoryId;
        SearchTerm = searchTerm;
    }
}
