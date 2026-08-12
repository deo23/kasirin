namespace KasirIn.Application.Transactions.Queries.GetSalesHistory;

using KasirIn.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetSalesHistoryQueryHandler : IRequestHandler<GetSalesHistoryQuery, List<TransactionDto>>
{
    private readonly IKasirInDbContext _context;

    public GetSalesHistoryQueryHandler(IKasirInDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionDto>> Handle(GetSalesHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.TransactionItems)
                .ThenInclude(ti => ti.Product)
            .Where(t => t.TenantId == request.TenantId);

        if (request.StartDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= request.EndDate.Value);
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                TenantId = t.TenantId,
                UserId = t.UserId,
                UserName = t.User != null ? t.User.FullName : string.Empty,
                InvoiceNumber = t.InvoiceNumber,
                TransactionDate = t.TransactionDate,
                TotalAmount = t.TotalAmount,
                PaidAmount = t.PaidAmount,
                ChangeAmount = t.ChangeAmount,
                PaymentMethod = t.PaymentMethod,
                TotalProfit = t.TransactionItems.Sum(item => item.Profit),
                Items = t.TransactionItems.Select(item => new TransactionItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product != null ? item.Product.Name : string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    CostPrice = item.CostPrice,
                    SubTotal = item.SubTotal,
                    Profit = item.Profit
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return transactions;
    }
}
