using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server.Internal;

internal class RndWebServer : IWebServer
{
    private readonly IServiceProvider _services;
    private readonly ILogger<RndWebServer> _logger;

    public RndWebServer(IServiceProvider services, ILogger<RndWebServer> logger)
    {
        _services = services;
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
            var connection = await socket.AcceptAsync(cancellationToken);
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var middleWares = scope.ServiceProvider.GetServices<IWeberverMiddleWare>().ToArray();
                    var headerParser = new HeaderParser();
                    var bytesRead = 0;
                    while (true)
                    {
                        var buffer = new byte[4096];
                        var read = await connection.ReceiveAsync(buffer, SocketFlags.None);
                        bytesRead += read;
                        if (read == 0)
                        {
                            break;
                        }

                        var hasHeaders = headerParser.Write(buffer);
                        if (!hasHeaders)
                        {
                            continue;
                        }

                        var context = new WebContext
                        {
                            Headers = new(headerParser.Headers[1..].GroupBy(x => x.Key).Select(x => x.First())),
                        };

                        if (context.Headers.ContainsKey("content-length") && long.TryParse(context.Headers["content-length"], out var length))
                        {
                            context.ContentLength = length;
                        }

                        if (context.ContentLength > 0)
                        {
                            
                        }
                        
                        var response = new ResponseContext(new WebResponse(connection));

                        foreach (var middleWare in middleWares)
                        {
                            await middleWare.ProcessAsync(context, response, cancellationToken);
                            if (response.StopProcessing)
                            {
                                connection.Close();
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in web server");
                }
            }, cancellationToken);
        }
    }

    private async Task ExecuteMiddlewares()
    {
    }
}