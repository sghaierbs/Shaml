using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;

namespace Shaml.Workflows.Elsa.Activities;

[Activity(
    "Shaml",
    "Assignments",
    "Waits until a Shaml assignment is completed.")]
public sealed class WaitForAssignmentCompleted : Activity
{
    public const string BookmarkName = "AssignmentCompleted";

    public Input<string> TaskCode { get; set; } = default!;

    protected override ValueTask ExecuteAsync(
        ActivityExecutionContext context)
    {
        var taskCode = TaskCode.Get(context);

        var correlationId =
            context.WorkflowExecutionContext.CorrelationId;

        if (!Guid.TryParse(correlationId, out var caseId))
        {
            throw new InvalidOperationException(
                $"Workflow correlation ID '{correlationId}' is not a valid Shaml CaseId.");
        }

        var stimulus = new AssignmentCompletedStimulus(
            caseId,
            taskCode);

        context.CreateBookmark(
            new CreateBookmarkArgs
            {
                BookmarkName = BookmarkName,
                Stimulus = stimulus,
                IncludeActivityInstanceId = false,
                AutoBurn = true
            });

        return ValueTask.CompletedTask;
    }
}