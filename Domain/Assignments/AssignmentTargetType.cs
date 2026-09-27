namespace Domain.Assignments;

public enum AssignmentTargetType
{
    /**
     * Exp: any specialist in riyadh center. (Work queue)
     */
    RoleQueue = 1,
    /**
     User x who is acting as Specialist in Riyadh center(this is mainly for internal users)
     because the same user can have multiple roles in the same center or the same role in multiple centers.
     (Work list)
     **/
    UserRole = 2, 
    /**
     * This is for public users
     */
    User = 3
}