using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace RndWebServer.Server;

public class RndWebServer : IWebServer
{
    private readonly List<IWeberverMiddleWare> _middleWares;
    private readonly ILogger<RndWebServer> _logger;

    public RndWebServer(List<IWeberverMiddleWare> middleWares, ILogger<RndWebServer> logger)
    {
        _middleWares = middleWares;
        _logger = logger;
    }

    public async Task StartAsync(int port, CancellationToken cancellationToken = default)
    {
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
        _logger.LogInformation("Webserver started on port {Port}", port);
        socket.Listen();
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var connection = await socket.AcceptAsync();
            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        using var requestStream = new MemoryStream();
                        var buffer = new byte[4096];
                        var read = await connection.ReceiveAsync(buffer, SocketFlags.None);
                        if (read == 0)
                        {
                            break;
                        }
                        else
                        {
                            requestStream.Write(buffer, (int)requestStream.Length, read);
                        }
                        var context = new WebContext();
                        var response = new ResponseContext();

                        foreach (var middleWare in _middleWares)
                        {
                            await middleWare.ProcessAsync(context, response, cancellationToken);
                            if (response.StopProcessing)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in web server");
                }
            });
        }
    }
}