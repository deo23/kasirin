namespace KasirIn.Application.Reports.Queries.GetProfitReport;

using KasirIn.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetProfitReportQueryHandler : IRequestHandler<GetProfitReportQuery, ProfitReportDto>
{
    private readonly IKasirInDbContext _context;

    public GetProfitReportQueryHandler(IKasirInDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitReportDto> Handle(GetProfitReportQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.TransactionItems)
            .Where(t => t.TenantId == request.TenantId
                        && t.TransactionDate >= request.StartDate
                        && t.TransactionDate <= request.EndDate)
            .ToListAsync(cancellationToken);

        int totalTransactions = transactions.Count;
        decimal totalRevenue = transactions.Sum(t => t.TotalAmount);
        decimal totalCogs = transactions.SelectMany(t => t.TransactionItems).Sum(item => item.CostPrice * item.Quantity);
        decimal netProfit = totalRevenue - totalCogs;

        return new ProfitReportDto
        {
            TenantId = request.TenantId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalTransactions = totalTransactions,
            TotalRevenue = totalRevenue,
            TotalCogs = totalCogs,
            NetProfit = netProfit
        };
    }
}
