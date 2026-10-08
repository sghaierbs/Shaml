using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;

namespace Application.Assignments.CreateDirectorApprovalAssignment;

public sealed class CreateDirectorApprovalAssignmentHandler
{
    private const string CenterDirectorRoleCode = "center-director";

    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICaseRepository _caseRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDirectorApprovalAssignmentHandler(
        IAssignmentRepository assignmentRepository,
        ICaseRepository caseRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _caseRepository = caseRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        CreateDirectorApprovalAssignmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingAssignment =
            await _assignmentRepository.GetByCaseAndTaskCodeAsync(
                command.CaseId,
                AssignmentTaskCodes.DirectorApproval,
                cancellationToken);

        if (existingAssignment is not null)
        {
            return;
        }

        var shamlCase =
            await _caseRepository.GetByIdAsync(
                command.CaseId,
                cancellationToken);

        if (shamlCase is null)
        {
            throw new NotFoundException(
                $"Case '{command.CaseId}' was not found.");
        }

        var centerDirectorRole =
            await _roleRepository.GetByCodeAsync(
                CenterDirectorRoleCode,
                cancellationToken);

        if (centerDirectorRole is null)
        {
            throw new NotFoundException(
                $"Role '{CenterDirectorRoleCode}' was not found.");
        }

        if (!centerDirectorRole.IsActive)
        {
            throw new InvalidOperationException(
                $"Role '{CenterDirectorRoleCode}' is inactive.");
        }

        var assignment =
            Assignment.CreateForRoleQueue(
                shamlCase.Id,
                AssignmentTaskCodes.DirectorApproval,
                centerDirectorRole.Id,
                shamlCase.CenterId,
                DateTime.UtcNow);

        await _assignmentRepository.AddAsync(
            assignment,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}