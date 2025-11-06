using CleanArch.Application.Common.Models;
using CleanArch.Application.Dashboard.DTOs;
using MediatR;

namespace CleanArch.Application.Dashboard.Queries.GetDashboardStats;

/// <summary>
/// Query para obtener estadísticas del dashboard
/// </summary>
public record GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>;
