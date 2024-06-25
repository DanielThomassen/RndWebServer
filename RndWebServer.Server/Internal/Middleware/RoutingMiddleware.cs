using Microsoft.Extensions.DependencyInjection;
using RndWebServer.Server.Infrastructure;
using RndWebServer.Server.Internal;

namespace RndWebServer.Server;

public class RoutingMiddleware : IWebserverMiddleWare
{
    private readonly RouteProvider _routeProvider;
    private readonly IServiceProvider _serviceProvider;

    public RoutingMiddleware(RouteProvider routeProvider, IServiceProvider serviceProvider)
    {
        _routeProvider = routeProvider;
        _serviceProvider = serviceProvider;
    }
    
    public Task ProcessAsync(RequestContext context, ResponseContext responseContext, CancellationToken cancellationToken)
    {
        var endpoint = _routeProvider.GetRoute(context.Path[1..], _serviceProvider);
        endpoint.Request = context;
        endpoint.Response = responseContext.Response;
        return context.Method switch
        {
            "GET" => endpoint.ProcessGet(),
            "POST" => endpoint.ProcessPost(),
            "PUT" => endpoint.ProcessPut(),
            "DELETE" => endpoint.ProcessDelete(),
            "PATCH" => endpoint.ProcessPatch(),
            _ => throw new InvalidOperationException($"Unsupported method {context.Method}")
        };
    }

    public void Configure(RequestProcessingOptions options)
    {
    }
}