namespace RndWebServer.Server;

public interface IWeberverMiddleWare
{
    Task ProcessAsync(WebContext context, ResponseContext responseContext,CancellationToken cancellationToken);
}