namespace RndWebServer.Server.Infrastructure;

public interface IWeberverMiddleWare
{
    Task ProcessAsync(WebContext context, ResponseContext responseContext,CancellationToken cancellationToken);

    public void Configure(RequestProcessingOptions options);
}