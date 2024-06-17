namespace RndWebServer.Server;

public interface IWebServer
{
    Task StartAsync(int port, CancellationToken cancellationToken);
}