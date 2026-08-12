namespace KasirIn.Application.Reports.Queries.GetProfitReport;

public record ProfitReportDto
{
    public Guid TenantId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int TotalTransactions { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal TotalCogs { get; init; }
    public decimal NetProfit { get; init; }
}
