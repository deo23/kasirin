namespace KasirIn.Application.Reports.Queries.GetProfitReport;

using MediatR;

public record GetProfitReportQuery(Guid TenantId, DateTime StartDate, DateTime EndDate) : IRequest<ProfitReportDto>;
