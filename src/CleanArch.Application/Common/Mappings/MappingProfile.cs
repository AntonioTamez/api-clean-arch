using AutoMapper;
using CleanArch.Application.Products.DTOs;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Common.Mappings;

/// <summary>
/// Perfil de AutoMapper para mapeos
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency));
    }
}
