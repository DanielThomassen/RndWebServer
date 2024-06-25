namespace RndWebServer.Server.Infrastructure;

public interface IWebserverMiddleWare
{
    Task ProcessAsync(RequestContext context, ResponseContext responseContext,CancellationToken cancellationToken);

    public void Configure(RequestProcessingOptions options);
}