using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArch.Infrastructure.Persistence.Configurations;

public class CapabilityConfiguration : IEntityTypeConfiguration<Capability>
{
    public void Configure(EntityTypeBuilder<Capability> builder)
    {
        builder.ToTable("Capabilities");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.Category)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.StartDate);

        builder.Property(c => c.EndDate);

        // Foreign Key
        builder.Property(c => c.ApplicationId)
            .IsRequired();

        // Relaciones
        builder.HasMany(c => c.BusinessRules)
            .WithOne()
            .HasForeignKey(br => br.CapabilityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.ModifiedAt);

        builder.Property(c => c.ModifiedBy)
            .HasMaxLength(100);

        // Ignore domain events
        builder.Ignore(c => c.DomainEvents);

        // Indexes
        builder.HasIndex(c => c.ApplicationId)
            .HasDatabaseName("IX_Capabilities_ApplicationId");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Capabilities_Status");

        builder.HasIndex(c => c.Priority)
            .HasDatabaseName("IX_Capabilities_Priority");

        builder.HasIndex(c => c.Category)
            .HasDatabaseName("IX_Capabilities_Category");
    }
}
