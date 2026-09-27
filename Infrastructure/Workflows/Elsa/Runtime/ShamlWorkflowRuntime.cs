using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;

namespace Infrastructure.Workflows.Elsa.Runtime;

public sealed class ShamlWorkflowRuntime
{
    private readonly IWorkflowRuntime _workflowRuntime;

    public ShamlWorkflowRuntime(IWorkflowRuntime workflowRuntime)
    {
        _workflowRuntime = workflowRuntime;
    }

    public async Task<ShamlWorkflowRunResult> RunPocAsync(CancellationToken cancellationToken = default)
    {
        var client = await _workflowRuntime.CreateClientAsync(cancellationToken);
        var correlationId = Guid.NewGuid().ToString();
        var result = await client.CreateAndRunInstanceAsync(
            new CreateAndRunWorkflowInstanceRequest
            {
                WorkflowDefinitionHandle = WorkflowDefinitionHandle.ByDefinitionId("shaml-poc"),

                CorrelationId = correlationId
            },
            cancellationToken);

        return new ShamlWorkflowRunResult(
            result.WorkflowInstanceId,
            correlationId,
            result.Status.ToString(),
            result.SubStatus.ToString());
    }
}

public sealed record ShamlWorkflowRunResult(
    string WorkflowInstanceId,
    string CorrelationId,
    string Status,
    string SubStatus);