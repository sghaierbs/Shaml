using Domain.Common;

namespace Domain.Identity;

public class Role : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    public PortalType Portal { get; private set; }
    public ScopeType ScopeType { get; private set; }

    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }

    private Role()
    {
    }

    private Role(
        Guid id,
        string code,
        string name,
        PortalType portal,
        ScopeType scopeType,
        bool isSystem) : base(id)
    {
        Code = code;
        Name = name;
        Portal = portal;
        ScopeType = scopeType;
        IsSystem = isSystem;
        IsActive = true;
    }

    public static Role Create(
        string code,
        string name,
        PortalType portal,
        ScopeType scopeType,
        bool isSystem = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Role code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Role name is required.",
                nameof(name));

        return new Role(
            Guid.NewGuid(),
            code.Trim().ToLowerInvariant(),
            name.Trim(),
            portal,
            scopeType,
            isSystem);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}