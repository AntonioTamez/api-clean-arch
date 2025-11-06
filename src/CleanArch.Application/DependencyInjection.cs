using System.Reflection;
using CleanArch.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application;

/// <summary>
/// Configuración de Dependency Injection para la capa Application
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Registrar MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            
            // Agregar behaviors
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // Registrar AutoMapper
        services.AddAutoMapper(assembly);

        // Registrar FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
