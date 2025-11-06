using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArch.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        // ProjectCode como owned type
        builder.OwnsOne(p => p.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("Code")
                .HasMaxLength(30)
                .IsRequired();

            code.HasIndex(c => c.Value)
                .IsUnique()
                .HasDatabaseName("IX_Projects_Code");
        });

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.Property(p => p.PlannedEndDate)
            .IsRequired(false);

        builder.Property(p => p.ActualEndDate)
            .IsRequired(false);

        builder.Property(p => p.ProjectManager)
            .HasMaxLength(100)
            .IsRequired();

        // Relaciones
        builder.HasMany(p => p.Applications)
            .WithOne()
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.ModifiedAt);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);

        // Ignore domain events (no persisted)
        builder.Ignore(p => p.DomainEvents);

        // Indexes
        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Projects_Status");

        builder.HasIndex(p => p.StartDate)
            .HasDatabaseName("IX_Projects_StartDate");
    }
}
