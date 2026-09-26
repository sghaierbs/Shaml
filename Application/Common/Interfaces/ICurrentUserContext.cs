namespace Application.Common.Interfaces;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }

    string? ExternalId { get; }

    Guid? UserId { get; }

    Guid? ActiveUserRoleId { get; }

    Guid? ActiveRoleId { get; }

    Guid? ActiveCenterId { get; }
}