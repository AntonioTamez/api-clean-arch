using CleanArch.Domain.Common;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Entidad que representa una versión específica de una página wiki
/// </summary>
public class WikiPageVersion : BaseEntity
{
    private WikiPageVersion() { } // EF Core

    internal WikiPageVersion(
        Guid wikiPageId,
        int versionNumber,
        string content,
        string changeSummary,
        string authorId)
    {
        WikiPageId = wikiPageId;
        VersionNumber = versionNumber;
        Content = content;
        ChangeSummary = changeSummary;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid WikiPageId { get; private set; }
    public int VersionNumber { get; private set; }
    public string Content { get; private set; } = null!;
    public string ChangeSummary { get; private set; } = null!;
    public string AuthorId { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public string? Changes { get; private set; } // JSON diff opcional

    public void SetChanges(string changes)
    {
        Changes = changes;
    }
}
