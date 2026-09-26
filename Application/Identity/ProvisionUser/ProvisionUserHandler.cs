using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Identity;

namespace Application.Identity.ProvisionUser;

public class ProvisionUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProvisionUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
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
        {
            throw new ConflictException(
                $"User with external identity '{command.ExternalId}' already exists.");
        }

        var publicRole =
            await _roleRepository.GetByIdAsync(
                SystemRoleIds.PublicUser,
                cancellationToken);

        if (publicRole is null)
        {
            throw new InvalidOperationException(
                "Public User system role is not configured.");
        }

        if (!publicRole.IsActive)
        {
            throw new InvalidOperationException(
                "Public User system role is inactive.");
        }

        var user = User.Create(
            command.ExternalId,
            command.FullName,
            command.Email);

        var publicUserRole = UserRole.Create(
            user.Id,
            publicRole,
            centerId: null,
            isDefault: true);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _userRoleRepository.AddAsync(
            publicUserRole,
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