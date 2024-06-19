using System.Collections.Specialized;

namespace RndWebServer.Server;

public sealed class WebContext
{
    public Dictionary<string, string> Headers { get; set; } = new();
    
    public string Query { get; internal set; }
    
    public string Path { get; internal set; }
    
    public string Method { get; internal set; }
    
    public Stream BodyStream { get; internal set; }
    public long ContentLength { get; internal set; }
    public NameValueCollection QueryParams { get; set; } = null!;
}