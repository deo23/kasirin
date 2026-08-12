namespace KasirIn.Api.Controllers;

using KasirIn.Application.Transactions.Commands.CreateTransaction;
using KasirIn.Application.Transactions.Queries.GetSalesHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public TransactionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<object>> CreateTransaction(
        [FromBody] CreateTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<TransactionDto>>> GetSalesHistory(
        [FromQuery] Guid tenantId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSalesHistoryQuery(tenantId, startDate, endDate);
        var history = await _sender.Send(query, cancellationToken);
        return Ok(history);
    }
}
