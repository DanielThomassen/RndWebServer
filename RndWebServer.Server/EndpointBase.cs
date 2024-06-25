using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server;


public abstract class EndpointBase : IEndpoint
{
    public IWebResponse Response { get; internal set; } = null!;
    public RequestContext Request { get; internal set; } = null!;
    
    public virtual Task ProcessPost()
    {
        return MethodNotAllowed();
    }

    public virtual  Task ProcessGet()
    {
        return MethodNotAllowed();
    }

    public virtual  Task ProcessDelete()
    {
        return MethodNotAllowed();
    }

    public virtual  Task ProcessPut()
    {
        return MethodNotAllowed();
    }

    public virtual  Task ProcessPatch()
    {
        return MethodNotAllowed();
    }

    protected Task NoContent()
    {
        return WriteResponse(string.Empty, 204);
    }
    
    protected Task Ok()
    {
        return WriteResponse(string.Empty, 200);
    }
    
    protected Task Ok(string content)
    {
        return WriteResponse(content, 200);
    }
    
    protected Task MethodNotAllowed()
    {
        return WriteResponse("Method Not Allowed", 405);
    }
    
    protected Task NotFound()
    {
        return WriteResponse("Not Found", 404);
    }

    protected async Task WriteResponse(string content, int statusCode)
    {
        Response.StatusCode = statusCode;
        await Response.WriteAsync(content);
    }
}