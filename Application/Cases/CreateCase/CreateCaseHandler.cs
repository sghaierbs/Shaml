using Application.Common.Interfaces;
using Domain.Cases;

namespace Application.Cases.CreateCase;

public sealed class CreateCaseHandler
{
    private readonly ICaseRepository _caseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCaseHandler(
        ICaseRepository caseRepository,
        IUnitOfWork unitOfWork)
    {
        _caseRepository = caseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCaseResult> HandleAsync(
        CreateCaseCommand command,
        CancellationToken cancellationToken = default)
    {
        var shamlCase = Case.Create(command.CaseNumber);

        await _caseRepository.AddAsync(
            shamlCase,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateCaseResult(
            shamlCase.Id,
            shamlCase.CaseNumber);
    }
}