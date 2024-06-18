using System.Net.Sockets;
using System.Text;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server.Internal;

internal class WebResponse : IWebResponse
{
    private readonly Socket _socket;
    private bool _writeStarted;
    private Dictionary<string, string> _headers = new();

    private static IReadOnlyDictionary<int, byte[]> _statusCodes => new Dictionary<int, byte[]>
    {
        [200] = "HTTP/1.1 200 OK\r\n"u8.ToArray(),
        [201] = "HTTP/1.1 201 Created\r\n"u8.ToArray(),
        [204] = "HTTP/1.1 204 No Content\r\n"u8.ToArray(),
        
        [301] = "HTTP/1.1 301 Moved Permanently\r\n"u8.ToArray(),
        [302] = "HTTP/1.1 302 Found\r\n"u8.ToArray(),
        [303] = "HTTP/1.1 303 See Other\r\n"u8.ToArray(),
        [304] = "HTTP/1.1 304 Not Modified\r\n"u8.ToArray(),
        [307] = "HTTP/1.1 307 Temporary Redirect\r\n"u8.ToArray(),
        [308] = "HTTP/1.1 308 Permanent Redirect\r\n"u8.ToArray(),
        
        [400] = "HTTP/1.1 400 Bad Request\r\n"u8.ToArray(),
        [401] = "HTTP/1.1 401 Unauthorized\r\n"u8.ToArray(),
        [403] = "HTTP/1.1 403 Forbidden\r\n"u8.ToArray(),
        [404] = "HTTP/1.1 404 Not Found\r\n"u8.ToArray(),
        [500] = "HTTP/1.1 500 Internal Server Error\r\n"u8.ToArray(),
        [501] = "HTTP/1.1 501 Not Implemented\r\n"u8.ToArray(),
        [502] = "HTTP/1.1 502 Bad Gateway\r\n"u8.ToArray(),
        
    };

    public int StatusCode { get; set; } = 200;
    
    public WebResponse(Socket socket)
    {
        _socket = socket;
    }
    
    public void Write(ReadOnlySpan<char> content)
    {
        Write(Encoding.UTF8.GetBytes(content.ToArray()), 0, content.Length);
    }
    
    public ValueTask<int> WriteAsync(string content)
    {
        return WriteAsync(Encoding.UTF8.GetBytes(content), default);
    }
    
    public void Write(byte[] buffer, int offset, int count)
    {
        if (!_writeStarted)
        {
            WriteHeaders();
        }
        _writeStarted = true;
        _socket.Send(buffer, offset, count, SocketFlags.None);
    }

    public async ValueTask<int> WriteAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        if (!_writeStarted)
        {
            await WriteHeadersAsync();
        }
        _writeStarted = true;
        Console.WriteLine(Encoding.UTF8.GetString(buffer), buffer.Length);
        var result = await _socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
        _socket.Send("\r\n"u8, SocketFlags.None);
        
        return result;
    }
    
    private void WriteHeaders()
    {
        if (_writeStarted)
        {
            return;
        }
        _socket.Send("\r\n"u8, SocketFlags.None);

        
        foreach (var (key, value) in _headers)
        {
            _socket.Send(Encoding.UTF8.GetBytes($"{key}: {value}\r\n"), SocketFlags.None);
        }
        _socket.Send("\r\n"u8, SocketFlags.None);
    }
    
    private async ValueTask WriteHeadersAsync()
    {
        if (_writeStarted)
        {
            return;
        }

        Console.WriteLine("Sending headers");
        Console.WriteLine(Encoding.UTF8.GetString(_statusCodes[StatusCode]));
        await _socket.SendAsync(_statusCodes[StatusCode], SocketFlags.None);
        foreach (var (key, value) in _headers)
        {
            Console.WriteLine($"{key}: {value}\r\n");
            await _socket.SendAsync(Encoding.UTF8.GetBytes($"{key}: {value}\r\n"), SocketFlags.None);
        }
        _socket.Send("\r\n"u8, SocketFlags.None);
    }
}