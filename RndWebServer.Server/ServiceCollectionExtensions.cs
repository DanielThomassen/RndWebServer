using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RndWebServer.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRouting(this IServiceCollection services)
    {
        return services.AddRouting(Assembly.GetEntryAssembly()!);
    }
    
    public static IServiceCollection AddRouting<TMarker>(this IServiceCollection services)
    {
        return services.AddRouting(typeof(TMarker).Assembly);
    }

    public static IServiceCollection AddRouting(this IServiceCollection services, Assembly assembly)
    {
        var endpoints = assembly.GetTypes().Where(
                x => x.IsClass && !x.IsAbstract && x.IsAssignableTo(typeof(EndpointBase))
            )
            .ToArray();
        foreach (var endpoint in endpoints)
        {
            services.AddTransient(endpoint);
        }

        return services;
    }
}