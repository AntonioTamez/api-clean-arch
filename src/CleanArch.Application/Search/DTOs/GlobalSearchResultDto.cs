namespace CleanArch.Application.Search.DTOs;

/// <summary>
/// Resultado de búsqueda global
/// </summary>
public class GlobalSearchResultDto
{
    public List<SearchItemDto> Projects { get; set; } = new();
    public List<SearchItemDto> Capabilities { get; set; } = new();
    public List<SearchItemDto> BusinessRules { get; set; } = new();
    public List<SearchItemDto> WikiPages { get; set; } = new();
    public int TotalResults { get; set; }
}

public class SearchItemDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
