using Microsoft.Extensions.DependencyInjection;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server.Internal;

public sealed class RouteProvider
{
    private Dictionary<string, Type> Routes { get; } = new ();

    public void AddRoute(string path, Type t)
    {
        if (!t.IsAssignableTo(typeof(EndpointBase)))
        {
            throw new InvalidOperationException($"Type {t.Name} is not assignable to {nameof(EndpointBase)}");
        }
        Routes.Add(path.Trim().ToLower(), t);
    }
    
    public void AddRoute<T>(string path) where T : EndpointBase
    {
        Routes.Add(path.Trim().ToLower(), typeof(T));
    }

    public EndpointBase GetRoute(string path, IServiceProvider services)
    {
        if (!Routes.TryGetValue(path.Trim().ToLower(), out var type))
        {
            throw new Exception($"No route found for path {path}");
        }

        return (EndpointBase)services.GetRequiredService(type);
    }
}

public static class WebServerConfigurationBuilderExtensions
{
    public static void AddRoute<T>(this WebServerConfigurationBuilder builder, string path) where T : EndpointBase
    {
        builder.Services.GetRequiredService<RouteProvider>().AddRoute<T>(path);
    }
}