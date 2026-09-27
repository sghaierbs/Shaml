using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;
using Domain.Cases;
using Domain.Centers;

namespace Application.Cases.CreateCase;

public sealed class CreateCaseHandler
{
    private const string SpecialistRoleCode = "specialist";

    private readonly ICenterRepository _centerRepository;
    private readonly ICaseRepository _caseRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCaseHandler(
        ICaseRepository caseRepository,
        IAssignmentRepository assignmentRepository,
        IRoleRepository roleRepository,
        ICenterRepository centerRepository,
        IUnitOfWork unitOfWork)
    {
        _caseRepository = caseRepository;
        _assignmentRepository = assignmentRepository;
        _roleRepository = roleRepository;
        _centerRepository = centerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCaseResult> HandleAsync(CreateCaseCommand command, CancellationToken cancellationToken = default)
    {
        var specialistRole = await _roleRepository.GetByCodeAsync(
            SpecialistRoleCode,
            cancellationToken);
        
        var center = await _centerRepository.GetByIdAsync(
            command.CenterId,
            cancellationToken);

        if (center is null)
        {
            throw new NotFoundException(
                $"Center '{command.CenterId}' was not found.");
        }

        if (center.Status != CenterStatus.Active)
        {
            throw new InvalidOperationException(
                $"Center '{command.CenterId}' is inactive.");
        }
        
        if (specialistRole is null)
        {
            throw new NotFoundException(
                $"Role '{SpecialistRoleCode}' was not found.");
        }

        if (!specialistRole.IsActive)
        {
            throw new InvalidOperationException(
                $"Role '{SpecialistRoleCode}' is inactive.");
        }

        var shamlCase = Case.Create(
            command.CaseNumber,
            command.CenterId);

        shamlCase.WaitForSpecialist();

        var assignment = Assignment.CreateForRoleQueue(
            shamlCase.Id,
            AssignmentTaskCodes.ReviewCase,
            specialistRole.Id,
            shamlCase.CenterId,
            DateTime.UtcNow);

        await _caseRepository.AddAsync(
            shamlCase,
            cancellationToken);

        await _assignmentRepository.AddAsync(
            assignment,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateCaseResult(
            shamlCase.Id,
            shamlCase.CaseNumber);
    }
}