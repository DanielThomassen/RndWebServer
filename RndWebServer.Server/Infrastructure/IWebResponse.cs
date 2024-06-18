namespace RndWebServer.Server.Infrastructure;

public interface IWebResponse
{
    void Write(ReadOnlySpan<char> content);
    void Write(byte[] buffer, int offset, int count);
    ValueTask<int> WriteAsync(string content);
    ValueTask<int> WriteAsync(byte[] buffer, CancellationToken cancellationToken);
    
    int StatusCode { get; set; }
}