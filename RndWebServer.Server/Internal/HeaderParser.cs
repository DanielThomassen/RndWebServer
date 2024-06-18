using System.Text;

namespace RndWebServer.Server.Internal;

internal class HeaderParser
{
    private readonly StringBuilder _keyBuilder = new();
    private readonly StringBuilder _valueBuilder = new();
    private readonly List<KeyValuePair<string, string>> _headers = new();
    private State _state = State.Key;
    
    public bool HeaderComplete { get; private set; }
    
    public KeyValuePair<string,string>[] Headers => _headers.ToArray();

    public long Offset { get; private set; } = 0;
    public long WriteOffset { get; private set; } = 0;
    
    public bool Write(ReadOnlySpan<byte> buffer)
    {
        if (HeaderComplete)
        {
            return HeaderComplete;
        }
        
        var value = Encoding.UTF8.GetString(buffer).AsSpan();

        WriteOffset = 0;
        
        foreach (var c in value)
        {
            if (HeaderComplete)
            {
                return HeaderComplete;
            }

            Offset += 1; // UTF8 is 8 bits per char, so 1 byte
            WriteOffset += 1; // UTF8 is 8 bits per char, so 1 byte
            
            if (c == '\r')
            {
                continue;
            }

            if (c == ':' && _state == State.Key)
            {
                _state = State.Value;
                continue;
            }

            if (c == '\n')
            {
                FlushHeader();
                _state = State.Key;
                continue;
            }
            switch (_state)
            {
                case State.Key:
                    _keyBuilder.Append(c);
                    break;
                case State.Value:
                    _valueBuilder.Append(c);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return HeaderComplete;
    }

    private void FlushAndSetState(State state, ReadOnlySpan<char> value, StringBuilder output)
    {
        output.Append(value);
        _state = state;
    }

    public void FlushHeader()
    {
        if (HeaderComplete)
        {
            return;
        }
        var key = _keyBuilder.ToString().ToLower().Trim();
        var val = _valueBuilder.ToString();
        
        HeaderComplete = string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(val);
        if (HeaderComplete)
        {
            return;
        }
        
        _headers.Add(new KeyValuePair<string, string>(key, val));
        _keyBuilder.Clear();
        _valueBuilder.Clear();
    }
    
    private enum State
    {
        Key,
        Value
    }
}