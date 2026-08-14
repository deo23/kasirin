using KasirIn.Application.Reports.Queries.ExportProfitReportToExcel;
using KasirIn.Application.Reports.Queries.GetProfitReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("profit")]
    public async Task<ActionResult<ProfitReportDto>> GetProfitReport(
        [FromQuery] Guid tenantId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProfitReportQuery(tenantId, startDate, endDate);
        var report = await _sender.Send(query, cancellationToken);
        return Ok(report);
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] Guid tenantId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportProfitReportToExcelQuery(tenantId, startDate, endDate);
        var bytes = await _sender.Send(query, cancellationToken);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "KasirIn_Laporan_Keuangan.xlsx");
    }
}
