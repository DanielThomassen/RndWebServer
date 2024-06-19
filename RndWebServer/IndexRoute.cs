using RndWebServer.Server;

namespace RndWebServer;

public class IndexRoute : EndpointBase
{
    public override async Task ProcessGet()
    {
        var name = "World";
        if (!string.IsNullOrWhiteSpace(Request.QueryParams["name"]))
        {
            name = Request.QueryParams["name"];
        }
        await Ok($"Hello {name}!");
    }
    
}
