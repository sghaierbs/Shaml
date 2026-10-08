namespace Application.Common.Interfaces;

public interface ICaseWorkflowService
{
    Task StartAsync(
        Guid caseId,
        CancellationToken cancellationToken = default);

    Task ResumeAssignmentCompletedAsync(
        Guid caseId,
        string taskCode,
        CancellationToken cancellationToken = default);
}