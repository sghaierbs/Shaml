using Infrastructure.Workflows.Elsa.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/workflow-poc")]
public sealed class WorkflowPocController : ControllerBase
{
    [HttpPost("run")]
    public async Task<ActionResult<ShamlWorkflowRunResult>> Run(
        [FromServices] ShamlWorkflowRuntime workflowRuntime,
        CancellationToken cancellationToken)
    {
        var result = await workflowRuntime.RunPocAsync(
            cancellationToken);

        return Ok(result);
    }
}