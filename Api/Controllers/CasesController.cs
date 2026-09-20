using Microsoft.AspNetCore.Mvc;
using Application.Cases.CreateCase;

namespace Api.Controllers;

[ApiController]
[Route("api/cases")]
public sealed class CasesController : ControllerBase
{
    private readonly CreateCaseHandler _createCaseHandler;

    public CasesController(CreateCaseHandler createCaseHandler)
    {
        _createCaseHandler = createCaseHandler;
    }

    [HttpPost]
    public async Task<ActionResult<CreateCaseResult>> Create(
        CreateCaseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCaseCommand(
            request.CaseNumber);

        var result = await _createCaseHandler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/cases/{result.Id}",
            result);
    }
}

public sealed record CreateCaseRequest(
    string CaseNumber);