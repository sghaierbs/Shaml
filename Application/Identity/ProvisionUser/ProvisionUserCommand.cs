namespace Application.Identity.ProvisionUser;

public sealed record ProvisionUserCommand(
    string ExternalId,
    string FullName,
    string? Email = null);