namespace KasirIn.Application.Products.Queries.GetProducts;

using KasirIn.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IKasirInDbContext _context;

    public GetProductsQueryHandler(IKasirInDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.TenantId == request.TenantId);

        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.SKU.ToLower().Contains(term));
        }

        var products = await query.Select(p => new ProductDto
        {
            Id = p.Id,
            TenantId = p.TenantId,
            CategoryId = p.CategoryId,
            CategoryName = p.Category != null ? p.Category.Name : string.Empty,
            Name = p.Name,
            SKU = p.SKU,
            CostPrice = p.CostPrice,
            SellingPrice = p.SellingPrice,
            StockQuantity = p.StockQuantity,
            MinStockThreshold = p.MinStockThreshold,
            ImageUrl = p.ImageUrl,
            IsLowStock = p.StockQuantity <= p.MinStockThreshold
        }).ToListAsync(cancellationToken);

        return products;
    }
}
