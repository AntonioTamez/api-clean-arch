using CleanArch.Application.Common.Models;
using CleanArch.Application.Search.DTOs;
using MediatR;

namespace CleanArch.Application.Search.Queries.GlobalSearch;

/// <summary>
/// Query para búsqueda global en todas las entidades
/// </summary>
public record GlobalSearchQuery(string SearchTerm, int MaxResultsPerType = 5) 
    : IRequest<Result<GlobalSearchResultDto>>;
