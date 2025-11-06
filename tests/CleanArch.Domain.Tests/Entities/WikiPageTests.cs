using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArch.Domain.Tests.Entities;

public class WikiPageTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var title = "API Integration Guide";
        var content = "# API Integration\n\nThis is a guide...";
        var category = "Guides";
        var authorId = "user-123";

        // Act
        var result = WikiPage.Create(title, content, category, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(title);
        result.Value.CurrentContent.Should().Be(content);
        result.Value.Category.Should().Be(category);
        result.Value.EntityType.Should().Be(WikiEntityType.General);
        result.Value.IsPublished.Should().BeFalse();
        result.Value.ViewCount.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ShouldFail(string invalidTitle)
    {
        // Act
        var result = WikiPage.Create(invalidTitle, "Content", "Category", "author-1");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("title");
    }

    [Fact]
    public void Create_ShouldGenerateSlugFromTitle()
    {
        // Arrange
        var title = "User Authentication Guide";

        // Act
        var result = WikiPage.Create(title, "Content", "Category", "author-1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Slug.Value.Should().Be("user-authentication-guide");
    }

    [Fact]
    public void Create_ShouldRaiseWikiPageCreatedEvent()
    {
        // Act
        var result = WikiPage.Create("Test Page", "Content", "Category", "author-1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.First().Should().BeOfType<WikiPageCreatedEvent>();
    }

    [Fact]
    public void Create_ShouldCreateInitialVersion()
    {
        // Act
        var result = WikiPage.Create("Test Page", "Initial content", "Category", "author-1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Versions.Should().HaveCount(1);
        result.Value.Versions.First().VersionNumber.Should().Be(1);
        result.Value.Versions.First().Content.Should().Be("Initial content");
    }

    [Fact]
    public void Update_WithValidContent_ShouldCreateNewVersion()
    {
        // Arrange
        var page = CreateValidWikiPage();
        var newContent = "Updated content";
        var changeSummary = "Fixed typos";

        // Act
        var result = page.Update(newContent, changeSummary, "author-2");

        // Assert
        result.IsSuccess.Should().BeTrue();
        page.CurrentContent.Should().Be(newContent);
        page.Versions.Should().HaveCount(2);
        page.Versions.Last().VersionNumber.Should().Be(2);
    }

    [Fact]
    public void Update_ShouldRaiseVersionCreatedEvent()
    {
        // Arrange
        var page = CreateValidWikiPage();
        page.ClearDomainEvents();

        // Act
        page.Update("New content", "Summary", "author-1");

        // Assert
        var versionEvent = page.DomainEvents.OfType<WikiPageVersionCreatedEvent>().FirstOrDefault();
        versionEvent.Should().NotBeNull();
        versionEvent!.VersionNumber.Should().Be(2);
    }

    [Fact]
    public void Publish_ShouldSetIsPublishedToTrue()
    {
        // Arrange
        var page = CreateValidWikiPage();

        // Act
        page.Publish();

        // Assert
        page.IsPublished.Should().BeTrue();
    }

    [Fact]
    public void Publish_ShouldRaiseWikiPagePublishedEvent()
    {
        // Arrange
        var page = CreateValidWikiPage();
        page.ClearDomainEvents();

        // Act
        page.Publish();

        // Assert
        page.DomainEvents.Should().ContainSingle();
        page.DomainEvents.First().Should().BeOfType<WikiPagePublishedEvent>();
    }

    [Fact]
    public void Unpublish_ShouldSetIsPublishedToFalse()
    {
        // Arrange
        var page = CreateValidWikiPage();
        page.Publish();

        // Act
        page.Unpublish();

        // Assert
        page.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void AddTag_WithValidTag_ShouldAddToCollection()
    {
        // Arrange
        var page = CreateValidWikiPage();
        var tag = "documentation";

        // Act
        var result = page.AddTag(tag);

        // Assert
        result.IsSuccess.Should().BeTrue();
        page.Tags.Should().Contain(tag);
    }

    [Fact]
    public void AddTag_WithDuplicateTag_ShouldFail()
    {
        // Arrange
        var page = CreateValidWikiPage();
        page.AddTag("test");

        // Act
        var result = page.AddTag("test");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void LinkToEntity_WithValidEntityId_ShouldSucceed()
    {
        // Arrange
        var page = CreateValidWikiPage();
        var entityId = Guid.NewGuid();

        // Act
        var result = page.LinkToEntity(WikiEntityType.Project, entityId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        page.EntityType.Should().Be(WikiEntityType.Project);
        page.EntityId.Should().Be(entityId);
    }

    [Fact]
    public void IncrementViewCount_ShouldIncreaseByOne()
    {
        // Arrange
        var page = CreateValidWikiPage();
        var initialCount = page.ViewCount;

        // Act
        page.IncrementViewCount();

        // Assert
        page.ViewCount.Should().Be(initialCount + 1);
    }

    [Fact]
    public void GetVersion_WithValidVersionNumber_ShouldReturnVersion()
    {
        // Arrange
        var page = CreateValidWikiPage();
        page.Update("V2 content", "Update", "author-1");

        // Act
        var version = page.GetVersion(1);

        // Assert
        version.Should().NotBeNull();
        version!.VersionNumber.Should().Be(1);
    }

    [Fact]
    public void GetVersion_WithInvalidVersionNumber_ShouldReturnNull()
    {
        // Arrange
        var page = CreateValidWikiPage();

        // Act
        var version = page.GetVersion(99);

        // Assert
        version.Should().BeNull();
    }

    private WikiPage CreateValidWikiPage()
    {
        return WikiPage.Create(
            "Test Page",
            "Test Content",
            "Test Category",
            "test-author").Value;
    }
}
