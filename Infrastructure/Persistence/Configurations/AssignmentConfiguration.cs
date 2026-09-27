using Domain.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class AssignmentConfiguration
    : IEntityTypeConfiguration<Assignment>
{
    public void Configure(
        EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TaskCode)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.TargetType)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CaseId)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.TargetRoleId);

        builder.Property(x => x.TargetCenterId);

        builder.Property(x => x.TargetUserRoleId);

        builder.Property(x => x.TargetUserId);

        builder.Property(x => x.ClaimedByUserRoleId);

        builder.Property(x => x.ClaimedAtUtc);

        builder.Property(x => x.CompletedAtUtc);

        builder.HasIndex(x => x.CaseId);

        builder.HasIndex(x => new
        {
            x.TargetType,
            x.TargetRoleId,
            x.TargetCenterId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.TargetUserRoleId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.TargetUserId,
            x.Status
        });
        
        builder.Property(x => x.ConcurrencyToken)
            .IsConcurrencyToken()
            .IsRequired();
    }
}