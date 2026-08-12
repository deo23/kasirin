namespace KasirIn.Api.Controllers;

using KasirIn.Application.Debts.Commands.CreateDebtRecord;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DebtsController : ControllerBase
{
    private readonly ISender _sender;

    public DebtsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<object>> CreateDebtRecord(
        [FromBody] CreateDebtRecordCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(new { id });
    }
}
