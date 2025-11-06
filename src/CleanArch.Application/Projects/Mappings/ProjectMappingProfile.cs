using AutoMapper;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Projects.Mappings;

/// <summary>
/// Perfil de mapeo para entidades Project
/// </summary>
public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count));

        CreateMap<Project, ProjectListItemDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count));
    }
}
