using Application.Common.Outbox;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/dev/outbox")]
public sealed class OutboxController : ControllerBase
{
    private readonly OutboxProcessor _outboxProcessor;

    public OutboxController(OutboxProcessor outboxProcessor)
    {
        _outboxProcessor = outboxProcessor;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process(CancellationToken cancellationToken)
    {
        await _outboxProcessor.ProcessAsync(cancellationToken);

        return Ok();
    }
}