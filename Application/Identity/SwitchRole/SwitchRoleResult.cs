namespace Application.Identity.SwitchRole;

public sealed record SwitchRoleResult(
    string AccessToken,
    DateTime ExpiresAtUtc);