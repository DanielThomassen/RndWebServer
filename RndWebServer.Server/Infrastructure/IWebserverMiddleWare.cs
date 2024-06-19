namespace RndWebServer.Server.Infrastructure;

public interface IWebserverMiddleWare
{
    Task ProcessAsync(WebContext context, ResponseContext responseContext,CancellationToken cancellationToken);

    public void Configure(RequestProcessingOptions options);
}