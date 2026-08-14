namespace KasirIn.Application.Reports.Queries.ExportProfitReportToExcel;

using MediatR;

public record ExportProfitReportToExcelQuery(
    Guid TenantId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<byte[]>;
