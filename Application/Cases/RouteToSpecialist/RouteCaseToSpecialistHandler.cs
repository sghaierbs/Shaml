using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;

namespace Application.Cases.RouteToSpecialist;

public sealed class RouteCaseToSpecialistHandler
{
    private readonly ICaseRepository _cases;
    private readonly IAssignmentRepository _assignments;
    private readonly IUnitOfWork _unitOfWork;

    public RouteCaseToSpecialistHandler(
        ICaseRepository cases,
        IAssignmentRepository assignments,
        IUnitOfWork unitOfWork)
    {
        _cases = cases;
        _assignments = assignments;
        _unitOfWork = unitOfWork;
    }

    public async Task<RouteCaseToSpecialistResult> HandleAsync(
        RouteCaseToSpecialistCommand command,
        CancellationToken cancellationToken = default)
    {
        var shamlCase = await _cases.GetByIdAsync(
            command.CaseId,
            cancellationToken);

        if (shamlCase is null)
        {
            throw new NotFoundException(
                $"Case '{command.CaseId}' was not found.");
        }

        shamlCase.WaitForSpecialist();

        var assignment = Assignment.CreateForRoleQueue(
            shamlCase.Id,
            AssignmentTaskCodes.ReviewCase,
            command.SpecialistRoleId,
            command.CenterId,
            DateTime.UtcNow);

        await _assignments.AddAsync(
            assignment,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new RouteCaseToSpecialistResult(
            shamlCase.Id,
            shamlCase.Status,
            assignment.Id);
    }
}