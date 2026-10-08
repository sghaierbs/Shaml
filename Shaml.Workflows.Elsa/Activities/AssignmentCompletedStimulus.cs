namespace Shaml.Workflows.Elsa.Activities;

public sealed record AssignmentCompletedStimulus(
    Guid CaseId,
    string TaskCode);