using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();
        
        
        builder.HasData(
            new
            {
                Id = SystemPermissionIds.CenterView,
                Code = "center.view",
                Description = "View Shaml centers"
            },
            new
            {
                Id = SystemPermissionIds.CenterManage,
                Code = "center.manage",
                Description = "Manage Shaml centers"
            },
            new
            {
                Id = SystemPermissionIds.CaseView,
                Code = "case.view",
                Description = "View cases"
            },
            new
            {
                Id = SystemPermissionIds.CaseAssign,
                Code = "case.assign",
                Description = "Assign cases"
            },
            new
            {
                Id = SystemPermissionIds.SessionView,
                Code = "session.view",
                Description = "View sessions"
            },
            new
            {
                Id = SystemPermissionIds.SessionSchedule,
                Code = "session.schedule",
                Description = "Schedule sessions"
            },
            new
            {
                Id = SystemPermissionIds.UserView,
                Code = "user.view",
                Description = "View users"
            },
            new
            {
                Id = SystemPermissionIds.UserAssignRole,
                Code = "user.assign-role",
                Description = "Assign roles to users"
            });
    }
}