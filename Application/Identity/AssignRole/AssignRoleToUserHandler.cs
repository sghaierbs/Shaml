using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Identity;

namespace Application.Identity.AssignRole;

public class AssignRoleToUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ICenterRepository _centerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleToUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ICenterRepository centerRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _centerRepository = centerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignRoleToUserResult> HandleAsync(
        AssignRoleToUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
            throw new NotFoundException(
                $"User '{command.UserId}' was not found.");

        var role = await _roleRepository.GetByIdAsync(
            command.RoleId,
            cancellationToken);

        if (role is null)
            throw new NotFoundException(
                $"Role '{command.RoleId}' was not found.");

        if (!role.IsActive)
            throw new InvalidOperationException(
                $"Role '{role.Code}' is inactive.");

        if (role.ScopeType == ScopeType.Center)
        {
            if (!command.CenterId.HasValue)
                throw new InvalidOperationException(
                    "A center is required for this role.");

            var center = await _centerRepository.GetByIdAsync(
                command.CenterId.Value,
                cancellationToken);

            if (center is null)
                throw new NotFoundException(
                    $"Center '{command.CenterId}' was not found.");
        }

        var alreadyAssigned =
            await _userRoleRepository.ExistsAsync(
                command.UserId,
                command.RoleId,
                command.CenterId,
                cancellationToken);

        if (alreadyAssigned)
            throw new ConflictException(
                "The role is already assigned to this user in this scope.");

        var userRole = UserRole.Create(
            user.Id,
            role,
            command.CenterId,
            command.IsDefault);

        await _userRoleRepository.AddAsync(
            userRole,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new AssignRoleToUserResult(
            userRole.Id,
            userRole.UserId,
            userRole.RoleId,
            userRole.CenterId);
    }
}