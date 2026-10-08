using Application.Common.Interfaces;
using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;
using Shaml.Workflows.Elsa.Activities;

namespace Shaml.Workflows.Elsa.Runtime;

public sealed class ElsaCaseWorkflowService : ICaseWorkflowService
{
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IStimulusSender _stimulusSender;

    public ElsaCaseWorkflowService(
        IWorkflowRuntime workflowRuntime,
        IStimulusSender stimulusSender)
    {
        _workflowRuntime = workflowRuntime;
        _stimulusSender = stimulusSender;
    }

    public async Task StartAsync(
        Guid caseId,
        CancellationToken cancellationToken = default)
    {
        var client =
            await _workflowRuntime.CreateClientAsync(
                cancellationToken);

        await client.CreateAndRunInstanceAsync(
            new CreateAndRunWorkflowInstanceRequest
            {
                WorkflowDefinitionHandle =
                    WorkflowDefinitionHandle.ByDefinitionId(
                        "shaml-case-workflow"),

                CorrelationId = caseId.ToString()
            },
            cancellationToken);
    }

    public async Task ResumeAssignmentCompletedAsync(
        Guid caseId,
        string taskCode,
        CancellationToken cancellationToken = default)
    {
        var stimulus = new AssignmentCompletedStimulus(
            caseId,
            taskCode);

        await _stimulusSender.SendAsync(
            WaitForAssignmentCompleted.BookmarkName,
            stimulus,
            cancellationToken: cancellationToken);
    }
}