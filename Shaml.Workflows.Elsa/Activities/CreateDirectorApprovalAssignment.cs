using Application.Assignments.CreateDirectorApprovalAssignment;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Shaml.Workflows.Elsa.Activities;

[Activity(
    "Shaml",
    "Assignments",
    "Creates the Center Director approval assignment.")]
public sealed class CreateDirectorApprovalAssignment : Activity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var correlationId =
            context.WorkflowExecutionContext.CorrelationId;

        if (!Guid.TryParse(correlationId, out var caseId))
        {
            throw new InvalidOperationException(
                $"Workflow correlation ID '{correlationId}' is not a valid Shaml CaseId.");
        }

        var handler =
            context.GetRequiredService<
                CreateDirectorApprovalAssignmentHandler>();

        await handler.HandleAsync(
            new CreateDirectorApprovalAssignmentCommand(caseId),
            context.CancellationToken);

        await context.CompleteActivityAsync();
    }
}