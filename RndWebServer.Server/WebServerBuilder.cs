using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server;

public class WebServerBuilder
{
    public ServiceCollection Services { get; } = new();
    
    public IWebServer Build()
    {
        var services = Services.BuildServiceProvider();
        var server = new Internal.RndWebServer(services, services.GetRequiredService<ILogger<Internal.RndWebServer>>());
        return server;
    }
    
    public void AddMiddleware<T>() where T : class, IWeberverMiddleWare
    {
        Services.AddSingleton<IWeberverMiddleWare, T>();
    }
}