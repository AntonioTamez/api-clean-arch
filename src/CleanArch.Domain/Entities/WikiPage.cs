using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Agregado raíz que representa una página wiki con versionado automático
/// </summary>
public class WikiPage : BaseAuditableEntity
{
    private readonly List<WikiPageVersion> _versions = new();
    private readonly List<string> _tags = new();

    private WikiPage() { } // EF Core

    private WikiPage(
        string title,
        Slug slug,
        string content,
        string category,
        string authorId)
    {
        Title = title;
        Slug = slug;
        CurrentContent = content;
        Category = category;
        EntityType = WikiEntityType.General;
        IsPublished = false;
        ViewCount = 0;

        // Crear versión inicial
        var initialVersion = new WikiPageVersion(Id, 1, content, "Initial version", authorId);
        _versions.Add(initialVersion);
    }

    public string Title { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public WikiEntityType EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public string CurrentContent { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public bool IsPublished { get; private set; }
    public int ViewCount { get; private set; }

    public IReadOnlyCollection<WikiPageVersion> Versions => _versions.AsReadOnly();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public static Result<WikiPage> Create(
        string title,
        string content,
        string category,
        string authorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<WikiPage>.Failure("Wiki page title cannot be empty");

        if (title.Length > 200)
            return Result<WikiPage>.Failure("Wiki page title cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(content))
            return Result<WikiPage>.Failure("Wiki page content cannot be empty");

        if (string.IsNullOrWhiteSpace(category))
            return Result<WikiPage>.Failure("Wiki page category cannot be empty");

        if (string.IsNullOrWhiteSpace(authorId))
            return Result<WikiPage>.Failure("Author ID cannot be empty");

        var slugResult = Slug.Create(title);
        if (slugResult.IsFailure)
            return Result<WikiPage>.Failure(slugResult.Error);

        var wikiPage = new WikiPage(title.Trim(), slugResult.Value, content.Trim(), category.Trim(), authorId);

        wikiPage.AddDomainEvent(new WikiPageCreatedEvent(
            wikiPage.Id,
            wikiPage.Title,
            wikiPage.Slug.Value));

        return Result<WikiPage>.Success(wikiPage);
    }

    public Result Update(string content, string changeSummary, string authorId)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure("Content cannot be empty");

        if (string.IsNullOrWhiteSpace(changeSummary))
            return Result.Failure("Change summary cannot be empty");

        if (string.IsNullOrWhiteSpace(authorId))
            return Result.Failure("Author ID cannot be empty");

        CurrentContent = content.Trim();

        var newVersionNumber = _versions.Count + 1;
        var newVersion = new WikiPageVersion(Id, newVersionNumber, content.Trim(), changeSummary.Trim(), authorId);
        _versions.Add(newVersion);

        AddDomainEvent(new WikiPageVersionCreatedEvent(
            Id,
            newVersionNumber,
            authorId));

        return Result.Success();
    }

    public void Publish()
    {
        if (IsPublished)
            return;

        IsPublished = true;

        AddDomainEvent(new WikiPagePublishedEvent(
            Id,
            Title));
    }

    public void Unpublish()
    {
        IsPublished = false;
    }

    public Result AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return Result.Failure("Tag cannot be empty");

        var normalizedTag = tag.Trim().ToLowerInvariant();

        if (_tags.Contains(normalizedTag))
            return Result.Failure("Tag already exists");

        if (_tags.Count >= 10)
            return Result.Failure("Cannot add more than 10 tags");

        _tags.Add(normalizedTag);
        return Result.Success();
    }

    public Result RemoveTag(string tag)
    {
        var normalizedTag = tag.Trim().ToLowerInvariant();

        if (!_tags.Contains(normalizedTag))
            return Result.Failure("Tag not found");

        _tags.Remove(normalizedTag);
        return Result.Success();
    }

    public Result LinkToEntity(WikiEntityType entityType, Guid entityId)
    {
        if (entityId == Guid.Empty)
            return Result.Failure("Entity ID cannot be empty");

        EntityType = entityType;
        EntityId = entityId;

        return Result.Success();
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public WikiPageVersion? GetVersion(int versionNumber)
    {
        return _versions.FirstOrDefault(v => v.VersionNumber == versionNumber);
    }

    public Result UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure("Title cannot be empty");

        if (title.Length > 200)
            return Result.Failure("Title cannot exceed 200 characters");

        Title = title.Trim();

        var slugResult = Slug.Create(title);
        if (slugResult.IsFailure)
            return Result.Failure(slugResult.Error);

        Slug = slugResult.Value;

        return Result.Success();
    }
}
