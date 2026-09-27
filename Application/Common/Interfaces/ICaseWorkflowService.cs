namespace Application.Common.Interfaces;

public interface ICaseWorkflowService
{
    Task StartAsync(Guid caseId, CancellationToken cancellationToken = default);
}