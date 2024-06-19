using System.Collections.Specialized;
using System.Web;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server;

internal sealed class QueryStringMiddleware : IWebserverMiddleWare
{
    private static readonly NameValueCollection Empty = new ();
    
    public Task ProcessAsync(WebContext context, ResponseContext responseContext, CancellationToken cancellationToken)
    {
        var index = context.Path.IndexOf('?');
        if (index == -1)
        {
            context.QueryParams = Empty;
            return Task.CompletedTask;
        }
        
        context.QueryParams = HttpUtility.ParseQueryString(context.Path[(index + 1)..]);
        context.Path = context.Path[..index];
        
        return Task.CompletedTask;
    }

    public void Configure(RequestProcessingOptions options)
    {
        
    }
}