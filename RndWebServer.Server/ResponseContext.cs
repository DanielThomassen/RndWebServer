namespace RndWebServer.Server;

public class ResponseContext
{
    private bool _writeStarted;
    
    private List<KeyValuePair<string, string>> _headers = new();

    public bool StopProcessing { get; set; }
    
    public void Write(ReadOnlySpan<char> content)
    {
        if (!_writeStarted)
        {
            WriteHeaders();
        }
        _writeStarted = true;
    }
    
    public async Task WriteAsync(string content)
    {
        if (!_writeStarted)
        {
            WriteHeaders();
        }
        _writeStarted = true;
    }
    
    public async Task AddHeaderAsync(string key, string value)
    {
        if (_writeStarted)
        {
            throw new InvalidOperationException("Cannot add headers after writing content.");
        }
        _headers.Add(new KeyValuePair<string, string>(key, value));
    }

    private void WriteInternal(string content)
    {
        
    }
    
    private void WriteHeaders()
    {
        foreach (var header in _headers)
        {
            WriteInternal($"{header.Key}: {header.Value}\r\n");
        }
    }
}