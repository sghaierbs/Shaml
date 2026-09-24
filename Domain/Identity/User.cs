using Domain.Common;

namespace Domain.Identity;

public class User : AggregateRoot
{
    // Identifier received from the external IAM system
    public string ExternalId { get; private set; } = null!;

    public string FullName { get; private set; } = null!;
    public string? Email { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
    }

    private User(
        Guid id,
        string externalId,
        string fullName,
        string? email) : base(id)
    {
        ExternalId = externalId;
        FullName = fullName;
        Email = email;

        Status = UserStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static User Create(
        string externalId,
        string fullName,
        string? email = null)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException(
                "External identity is required.",
                nameof(externalId));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(
                "Full name is required.",
                nameof(fullName));

        return new User(
            Guid.NewGuid(),
            externalId.Trim(),
            fullName.Trim(),
            email?.Trim());
    }

    public void Activate()
    {
        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }
}