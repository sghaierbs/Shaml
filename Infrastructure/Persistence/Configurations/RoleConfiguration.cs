using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Portal)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ScopeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsSystem)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
        
        builder.HasData(
            new
            {
                Id = SystemRoleIds.CenterDirector,
                Code = "center-director",
                Name = "Center Director",
                Portal = PortalType.Internal,
                ScopeType = ScopeType.Center,
                IsSystem = true,
                IsActive = true
            },
            new
            {
                Id = SystemRoleIds.Specialist,
                Code = "specialist",
                Name = "Specialist",
                Portal = PortalType.Internal,
                ScopeType = ScopeType.Center,
                IsSystem = true,
                IsActive = true
            },
            new
            {
                Id = SystemRoleIds.OperationsSupervisor,
                Code = "operations-supervisor",
                Name = "Operations Supervisor",
                Portal = PortalType.Internal,
                ScopeType = ScopeType.Organization,
                IsSystem = true,
                IsActive = true
            });
    }
}