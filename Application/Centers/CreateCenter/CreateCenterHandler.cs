using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Centers;

namespace Application.Centers.CreateCenter;

public sealed class CreateCenterHandler
{
    private readonly ICenterRepository _centers;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCenterHandler(
        ICenterRepository centers,
        IUnitOfWork unitOfWork)
    {
        _centers = centers;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCenterResult> HandleAsync(
        CreateCenterCommand command,
        CancellationToken cancellationToken = default)
    {
        var codeExists = await _centers.ExistsByCodeAsync(
            command.Code,
            cancellationToken);

        if (codeExists)
        {
            throw new ConflictException(
                $"Center with code '{command.Code}' already exists.");
        }

        var center = Center.Create(
            command.Code,
            command.Name,
            command.Region,
            command.City,
            command.Address,
            command.Phone,
            command.Email);

        await _centers.AddAsync(
            center,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateCenterResult(
            center.Id,
            center.Code,
            center.Name,
            center.Region,
            center.City);
    }
}