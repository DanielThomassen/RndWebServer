namespace RndWebServer.Server;

public class RequestProcessingOptions
{
    /// <summary>
    /// Enables reading the request body more than once
    /// </summary>
    public bool EnableMultipleReads { get; set; }
    
    internal RequestProcessingOptions()
    {
    }
}