namespace Domain.Identity;

public static class SystemPermissionIds
{
    public static readonly Guid CenterView =
        Guid.Parse("10000000-0000-0000-0000-000000000001");

    public static readonly Guid CenterManage =
        Guid.Parse("10000000-0000-0000-0000-000000000002");

    public static readonly Guid CaseView =
        Guid.Parse("10000000-0000-0000-0000-000000000003");

    public static readonly Guid CaseAssign =
        Guid.Parse("10000000-0000-0000-0000-000000000004");

    public static readonly Guid SessionView =
        Guid.Parse("10000000-0000-0000-0000-000000000005");

    public static readonly Guid SessionSchedule =
        Guid.Parse("10000000-0000-0000-0000-000000000006");

    public static readonly Guid UserView =
        Guid.Parse("10000000-0000-0000-0000-000000000007");

    public static readonly Guid UserAssignRole =
        Guid.Parse("10000000-0000-0000-0000-000000000008");
}