using Api.Contracts.Cases;
using Microsoft.AspNetCore.Mvc;
using Application.Cases.CreateCase;
using Application.Cases.RouteToSpecialist;

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
            request.CaseNumber,
            request.CenterId);

        var result = await _createCaseHandler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/cases/{result.Id}",
            result);
    }
    
    [HttpPost("{caseId:guid}/route-to-specialist")]
    public async Task<IActionResult> RouteToSpecialist(Guid caseId, RouteToSpecialistRequest request, [FromServices] RouteCaseToSpecialistHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RouteCaseToSpecialistCommand(
                caseId,
                request.SpecialistRoleId,
                request.CenterId),
            cancellationToken);

        return Ok(result);
    }
}

public sealed record CreateCaseRequest(
    string CaseNumber,
    Guid CenterId);