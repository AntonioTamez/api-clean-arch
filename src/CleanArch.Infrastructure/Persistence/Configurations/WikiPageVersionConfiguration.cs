using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArch.Infrastructure.Persistence.Configurations;

public class WikiPageVersionConfiguration : IEntityTypeConfiguration<WikiPageVersion>
{
    public void Configure(EntityTypeBuilder<WikiPageVersion> builder)
    {
        builder.ToTable("WikiPageVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.WikiPageId)
            .IsRequired();

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.Content)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(v => v.ChangeSummary)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(v => v.AuthorId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.Changes)
            .HasColumnType("nvarchar(max)");

        // Ignore domain events
        builder.Ignore(v => v.DomainEvents);

        // Indexes
        builder.HasIndex(v => v.WikiPageId)
            .HasDatabaseName("IX_WikiPageVersions_WikiPageId");

        builder.HasIndex(v => new { v.WikiPageId, v.VersionNumber })
            .IsUnique()
            .HasDatabaseName("IX_WikiPageVersions_WikiPageId_VersionNumber");

        builder.HasIndex(v => v.CreatedAt)
            .HasDatabaseName("IX_WikiPageVersions_CreatedAt");
    }
}
