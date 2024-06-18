namespace RndWebServer.Server.Infrastructure;

public interface IWebServer
{
    Task StartAsync(int port, CancellationToken cancellationToken);
}