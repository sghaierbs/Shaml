using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Cases;

namespace Infrastructure.Persistence.Configurations;

public sealed class CaseConfiguration :
    IEntityTypeConfiguration<Case>
{
    public void Configure(
        EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CaseNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.CaseNumber)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}