namespace Application.Identity.SwitchRole;

public sealed record SwitchRoleCommand(
    string ExternalId,
    Guid UserRoleId);