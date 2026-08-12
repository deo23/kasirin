namespace KasirIn.Application.Transactions.Queries.GetSalesHistory;

using MediatR;

public record GetSalesHistoryQuery : IRequest<List<TransactionDto>>
{
    public Guid TenantId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }

    public GetSalesHistoryQuery(Guid tenantId, DateTime? startDate = null, DateTime? endDate = null)
    {
        TenantId = tenantId;
        StartDate = startDate;
        EndDate = endDate;
    }
}
