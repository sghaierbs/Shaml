namespace Application.Identity.Permissions;

public interface IPermissionService
{
    Task<IReadOnlyCollection<string>> GetPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<bool> HasPermissionAsync(
        Guid roleId,
        string permissionCode,
        CancellationToken cancellationToken = default);
}