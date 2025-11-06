using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArch.Infrastructure.Persistence.Configurations;

public class BusinessRuleConfiguration : IEntityTypeConfiguration<BusinessRule>
{
    public void Configure(EntityTypeBuilder<BusinessRule> builder)
    {
        builder.ToTable("BusinessRules");

        builder.HasKey(br => br.Id);

        // RuleCode como owned type
        builder.OwnsOne(br => br.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("Code")
                .HasMaxLength(20)
                .IsRequired();

            code.HasIndex(c => c.Value)
                .IsUnique()
                .HasDatabaseName("IX_BusinessRules_Code");
        });

        builder.Property(br => br.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(br => br.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(br => br.RuleType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(br => br.Implementation)
            .HasMaxLength(4000);

        builder.Property(br => br.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(br => br.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Examples como colección
        builder.Property<List<string>>("_examples")
            .HasColumnName("Examples")
            .HasConversion(
                v => string.Join("||", v),
                v => v.Split("||", StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(4000);

        // Foreign Key
        builder.Property(br => br.CapabilityId)
            .IsRequired();

        // Audit fields
        builder.Property(br => br.CreatedAt)
            .IsRequired();

        builder.Property(br => br.CreatedBy)
            .HasMaxLength(100);

        builder.Property(br => br.ModifiedAt);

        builder.Property(br => br.ModifiedBy)
            .HasMaxLength(100);

        // Ignore domain events
        builder.Ignore(br => br.DomainEvents);

        // Indexes
        builder.HasIndex(br => br.CapabilityId)
            .HasDatabaseName("IX_BusinessRules_CapabilityId");

        builder.HasIndex(br => br.Status)
            .HasDatabaseName("IX_BusinessRules_Status");

        builder.HasIndex(br => br.RuleType)
            .HasDatabaseName("IX_BusinessRules_RuleType");
    }
}
