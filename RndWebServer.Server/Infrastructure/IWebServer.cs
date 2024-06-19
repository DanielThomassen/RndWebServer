using RndWebServer.Server.Internal;

namespace RndWebServer.Server.Infrastructure;

public interface IWebServer
{
    Task StartAsync(int port, CancellationToken cancellationToken);

    void Configure(Action<WebServerConfigurationBuilder> configure);
}