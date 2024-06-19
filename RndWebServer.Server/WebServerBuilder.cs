using Microsoft.Extensions.DependencyInjection;
using RndWebServer.Server.Infrastructure;
using RndWebServer.Server.Internal;

namespace RndWebServer.Server;

public sealed class WebServerBuilder
{
    public ServiceCollection Services { get; } = new();

    public WebServerBuilder()
    {
        Services.AddTransient<HeaderParser>();
        Services.AddSingleton<IWebServer, Internal.RndWebServer>();
        Services.AddSingleton<RouteProvider>();
        AddMiddleware<QueryStringMiddleware>();
        AddMiddleware<RoutingMiddleware>();
    }
    
    public IWebServer Build()
    {
        var services = Services.BuildServiceProvider();
        return services.GetRequiredService<IWebServer>();
    }
    
    public void AddMiddleware<T>() where T : class, IWebserverMiddleWare
    {
        Services.AddSingleton<IWebserverMiddleWare, T>();
    }
}