using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server;

public class ResponseContext
{
    public IWebResponse Response { get; }

    
    public ResponseContext(IWebResponse response)
    {
        Response = response;
    }

    public bool StopProcessing { get; set; }
}