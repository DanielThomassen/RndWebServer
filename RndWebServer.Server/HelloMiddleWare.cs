using Microsoft.Extensions.Logging;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server;

public class HelloMiddleWare : IWeberverMiddleWare
{
    private readonly ILogger<HelloMiddleWare> _logger;

    public HelloMiddleWare(ILogger<HelloMiddleWare> logger)
    {
        _logger = logger;
    }
    
    public async Task ProcessAsync(WebContext context, ResponseContext responseContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hello World middleware activated");
        await responseContext.Response.WriteAsync("Hello World!");
        responseContext.StopProcessing = true;
    }

    public void Configure(RequestProcessingOptions options)
    {
        options.EnableMultipleReads = true;
    }
}