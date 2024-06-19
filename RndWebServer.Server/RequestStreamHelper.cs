using System.Net.Sockets;

namespace RndWebServer.Server;

public static class RequestStreamHelper
{

    public static Stream CreateRequestStream(Socket socket, long contentLength, RequestProcessingOptions options)
    {
        if (!options.EnableMultipleReads)
        {
            return new RequestStream(socket, contentLength);
        }
        else
        {
            throw new NotImplementedException("Not yet supported");
        }

        return null!;
    }

    private class RepeatableRequestStream : Stream
    {
        private readonly Socket _socket;
        private MemoryStream _cache = new MemoryStream();

        public RepeatableRequestStream(Socket socket)
        {
            _socket = socket;
        }
        
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var b = buffer[offset..(offset + count)].AsSpan();
            _socket.Receive(b);
            _cache.Write(b);
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position
        {
            get => _cache.Position;
            set => _cache.Position = value;
        }
    }

    private class RequestStream : Stream
    {
        private readonly Socket _socket;

        public RequestStream(Socket socket, long contentLength)
        {
            Length = contentLength;
            _socket = socket;
        }
        
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var b = buffer[offset..(offset + count)].AsSpan();
            return _socket.Receive(b);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length { get; }

        public override long Position { get; set; }
    }
}

