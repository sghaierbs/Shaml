using System.Text.Json;
using Application.Assignments.CompleteAssignment;
using Application.Cases.CreateCase;
using Application.Common.Interfaces;
using Domain.Common.Outbox;

namespace Application.Common.Outbox;

public sealed class OutboxProcessor
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ICaseWorkflowService _caseWorkflowService;
    private readonly IUnitOfWork _unitOfWork;

    public OutboxProcessor(
        IOutboxRepository outboxRepository,
        ICaseWorkflowService caseWorkflowService,
        IUnitOfWork unitOfWork)
    {
        _outboxRepository = outboxRepository;
        _caseWorkflowService = caseWorkflowService;
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessAsync(
        CancellationToken cancellationToken = default)
    {
        var messages =
            await _outboxRepository.GetUnprocessedAsync(
                20,
                cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(
                    message,
                    cancellationToken);

                message.MarkProcessed(
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                message.MarkFailed(
                    ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        switch (message.Type)
        {
            case OutboxMessageTypes.CaseCreated:
            {
                var payload =
                    JsonSerializer.Deserialize<CaseCreatedOutboxPayload>(
                        message.Payload)
                    ?? throw new InvalidOperationException(
                        "Invalid case.created outbox payload.");

                await _caseWorkflowService.StartAsync(
                    payload.CaseId,
                    cancellationToken);

                break;
            }

            case OutboxMessageTypes.AssignmentCompleted:
            {
                var payload =
                    JsonSerializer.Deserialize<AssignmentCompletedOutboxPayload>(
                        message.Payload)
                    ?? throw new InvalidOperationException(
                        "Invalid assignment.completed outbox payload.");

                await _caseWorkflowService.ResumeAssignmentCompletedAsync(
                    payload.CaseId,
                    payload.TaskCode,
                    cancellationToken);

                break;
            }

            default:
                throw new InvalidOperationException(
                    $"Unsupported outbox message type '{message.Type}'.");
        }
    }
}