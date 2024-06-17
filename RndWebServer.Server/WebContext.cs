namespace RndWebServer.Server;

public class WebContext
{
    Dictionary<string, string> Headers { get; set; } = new();
    
    public string Query { get; internal set; }
    
    public string Path { get; internal set; }
    
    public string Method { get; internal set; }
    
    public Stream BodyStream { get; internal set; }
}