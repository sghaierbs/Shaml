namespace Domain.Identity;

public static class SystemRoleIds
{
    public static readonly Guid CenterDirector =
        Guid.Parse("20000000-0000-0000-0000-000000000001");

    public static readonly Guid Specialist =
        Guid.Parse("20000000-0000-0000-0000-000000000002");

    public static readonly Guid OperationsSupervisor =
        Guid.Parse("20000000-0000-0000-0000-000000000003");
    
    public static readonly Guid PublicUser =
        Guid.Parse("20000000-0000-0000-0000-000000000004");
}