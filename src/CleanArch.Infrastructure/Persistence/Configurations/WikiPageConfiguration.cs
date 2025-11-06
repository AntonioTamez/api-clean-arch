using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArch.Infrastructure.Persistence.Configurations;

public class WikiPageConfiguration : IEntityTypeConfiguration<WikiPage>
{
    public void Configure(EntityTypeBuilder<WikiPage> builder)
    {
        builder.ToTable("WikiPages");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .HasMaxLength(200)
            .IsRequired();

        // Slug como owned type
        builder.OwnsOne(w => w.Slug, slug =>
        {
            slug.Property(s => s.Value)
                .HasColumnName("Slug")
                .HasMaxLength(100)
                .IsRequired();

            slug.HasIndex(s => s.Value)
                .IsUnique()
                .HasDatabaseName("IX_WikiPages_Slug");
        });

        builder.Property(w => w.EntityType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.EntityId);

        builder.Property(w => w.CurrentContent)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(w => w.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(w => w.IsPublished)
            .IsRequired();

        builder.Property(w => w.ViewCount)
            .IsRequired();

        // Tags como colección
        builder.Property<List<string>>("_tags")
            .HasColumnName("Tags")
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(500);

        // Relaciones
        builder.HasMany(w => w.Versions)
            .WithOne()
            .HasForeignKey(v => v.WikiPageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.CreatedBy)
            .HasMaxLength(100);

        builder.Property(w => w.ModifiedAt);

        builder.Property(w => w.ModifiedBy)
            .HasMaxLength(100);

        // Ignore domain events
        builder.Ignore(w => w.DomainEvents);

        // Indexes
        builder.HasIndex(w => w.EntityType)
            .HasDatabaseName("IX_WikiPages_EntityType");

        builder.HasIndex(w => w.EntityId)
            .HasDatabaseName("IX_WikiPages_EntityId");

        builder.HasIndex(w => w.IsPublished)
            .HasDatabaseName("IX_WikiPages_IsPublished");

        builder.HasIndex(w => w.Category)
            .HasDatabaseName("IX_WikiPages_Category");
    }
}
