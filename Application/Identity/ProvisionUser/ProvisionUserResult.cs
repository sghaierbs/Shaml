namespace Application.Identity.ProvisionUser;

public sealed record ProvisionUserResult(
    Guid UserId,
    string ExternalId,
    string FullName,
    string? Email);