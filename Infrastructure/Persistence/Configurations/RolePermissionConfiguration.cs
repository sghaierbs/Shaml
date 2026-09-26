using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoleId)
            .IsRequired();

        builder.Property(x => x.PermissionId)
            .IsRequired();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
            {
                x.RoleId,
                x.PermissionId
            })
            .IsUnique();
        
        builder.HasData(
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                RoleId = SystemRoleIds.PublicUser,
                PermissionId = SystemPermissionIds.CaseView
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                RoleId = SystemRoleIds.PublicUser,
                PermissionId = SystemPermissionIds.SessionView
            });
    }
}