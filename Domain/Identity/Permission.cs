using Domain.Common;

namespace Domain.Identity;

public class Permission : Entity
{
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Permission()
    {
    }

    private Permission(
        Guid id,
        string code,
        string description) : base(id)
    {
        Code = code;
        Description = description;
    }

    public static Permission Create(
        string code,
        string description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Permission code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Permission description is required.",
                nameof(description));

        return new Permission(
            Guid.NewGuid(),
            code.Trim().ToLowerInvariant(),
            description.Trim());
    }
}