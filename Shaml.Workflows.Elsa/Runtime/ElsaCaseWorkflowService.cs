using Application.Common.Interfaces;
using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;

namespace Shaml.Workflows.Elsa.Runtime;

public sealed class ElsaCaseWorkflowService : ICaseWorkflowService
{
    private readonly IWorkflowRuntime _workflowRuntime;

    public ElsaCaseWorkflowService(IWorkflowRuntime workflowRuntime)
    {
        _workflowRuntime = workflowRuntime;
    }

    public async Task StartAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var client = await _workflowRuntime.CreateClientAsync(cancellationToken);

        await client.CreateAndRunInstanceAsync(
            new CreateAndRunWorkflowInstanceRequest
            {
                WorkflowDefinitionHandle = WorkflowDefinitionHandle.ByDefinitionId("shaml-case-workflow"),
                CorrelationId = caseId.ToString()
            },
            cancellationToken);
    }
}