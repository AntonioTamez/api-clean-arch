using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArch.Infrastructure.Persistence.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<CleanArch.Domain.Entities.Application>
{
    public void Configure(EntityTypeBuilder<CleanArch.Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // ApplicationVersion como owned type
        builder.OwnsOne(a => a.Version, version =>
        {
            version.Property(v => v.Major)
                .HasColumnName("VersionMajor")
                .IsRequired();

            version.Property(v => v.Minor)
                .HasColumnName("VersionMinor")
                .IsRequired();

            version.Property(v => v.Patch)
                .HasColumnName("VersionPatch")
                .IsRequired();
        });

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.TechnologyStack)
            .HasMaxLength(500);

        builder.Property(a => a.StartDate);

        builder.Property(a => a.EndDate);

        // Foreign Key
        builder.Property(a => a.ProjectId)
            .IsRequired();

        // Relaciones
        builder.HasMany(a => a.Capabilities)
            .WithOne()
            .HasForeignKey(c => c.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100);

        builder.Property(a => a.ModifiedAt);

        builder.Property(a => a.ModifiedBy)
            .HasMaxLength(100);

        // Ignore domain events
        builder.Ignore(a => a.DomainEvents);

        // Indexes
        builder.HasIndex(a => a.ProjectId)
            .HasDatabaseName("IX_Applications_ProjectId");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Applications_Status");

        builder.HasIndex(a => a.Name)
            .HasDatabaseName("IX_Applications_Name");
    }
}
