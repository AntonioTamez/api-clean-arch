using ApiCleanArch.Application.Users;
using ApiCleanArch.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCleanArch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository>(_ => new InMemoryUserRepository(InMemoryUserSeed.Records));

        return services;
    }
}
