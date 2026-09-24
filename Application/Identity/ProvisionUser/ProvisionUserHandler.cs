using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Identity;

namespace Application.Identity.ProvisionUser;

public class ProvisionUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProvisionUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProvisionUserResult> HandleAsync(
        ProvisionUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingUser =
            await _userRepository.GetByExternalIdAsync(
                command.ExternalId,
                cancellationToken);

        if (existingUser is not null)
            throw new ConflictException(
                $"User with external identity '{command.ExternalId}' already exists.");

        var user = User.Create(
            command.ExternalId,
            command.FullName,
            command.Email);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new ProvisionUserResult(
            user.Id,
            user.ExternalId,
            user.FullName,
            user.Email);
    }
}