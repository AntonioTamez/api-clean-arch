using ApiCleanArch.Application.Users.Create;
using ApiCleanArch.Application.Users.Delete;
using ApiCleanArch.Application.Users.Get;
using ApiCleanArch.Application.Users.List;
using ApiCleanArch.Application.Users.Update;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCleanArch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ListUsersHandler>();
        services.AddScoped<GetUserHandler>();
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<UpdateUserHandler>();
        services.AddScoped<DeleteUserHandler>();

        return services;
    }
}
