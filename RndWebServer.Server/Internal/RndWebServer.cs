using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RndWebServer.Server.Infrastructure;

namespace RndWebServer.Server.Internal;

internal sealed class RndWebServer : IWebServer
{
    private readonly IServiceProvider _services;
    private readonly ILogger<RndWebServer> _logger;

    public RndWebServer(IServiceProvider services, ILogger<RndWebServer> logger)
    {
        _services = services;
        _logger = logger;
    }

    public void Configure(Action<WebServerConfigurationBuilder> configure)
    {
        var server = new WebServerConfigurationBuilder { Services = _services };
        configure?.Invoke(server);
    }

    public async Task StartAsync(int port, CancellationToken cancellationToken = default)
    {
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
        _logger.LogInformation("Webserver started on port {Port}", port);
        socket.Listen();
        await RunAsync(cancellationToken, socket);
    }

    private async Task RunAsync(CancellationToken cancellationToken, Socket socket)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var connection = await socket.AcceptAsync(cancellationToken);
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var middleWares = scope.ServiceProvider.GetServices<IWebserverMiddleWare>().ToArray();

                    var options = new RequestProcessingOptions();
                    foreach (var middleWare in middleWares)
                    {
                        middleWare.Configure(options);
                    }

                    var headerParser = scope.ServiceProvider.GetRequiredService<HeaderParser>();
                    await ReadHeaders(connection, headerParser);

                    var httpInfoLine = headerParser.Headers[0].Key.Split(' ', 3);

                    var context = new WebContext
                    {
                        Method = httpInfoLine[0].ToUpper().Trim(),
                        Path = httpInfoLine[1],
                        Headers = new(headerParser.Headers[1..].GroupBy(x => x.Key).Select(x => x.First())),
                    };

                    if (context.Headers.ContainsKey("content-length") &&
                        long.TryParse(context.Headers["content-length"], out var length))
                    {
                        context.ContentLength = length;
                    }

                    context.BodyStream =
                        RequestStreamHelper.CreateRequestStream(connection, context.ContentLength, options);

                    var response = new ResponseContext(new WebResponse(connection));

                    foreach (var middleWare in middleWares)
                    {
                        await middleWare.ProcessAsync(context, response, cancellationToken);
                        if (response.StopProcessing)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in web server");
                }
                finally
                {
                    connection.Close();
                }
            }, cancellationToken);
        }
    }

    private static async Task ReadHeaders(Socket connection, HeaderParser headerParser)
    {
        while (true)
        {
            var buffer = new byte[4096];
            var read = await connection.ReceiveAsync(buffer, SocketFlags.None);
            if (read == 0)
            {
                break;
            }

            var hasHeaders = headerParser.Write(buffer);
            if (!hasHeaders)
            {
                continue;
            }

            break;
        }
    }
}

public sealed class WebServerConfigurationBuilder
{
    public IServiceProvider Services { get; init; }

    internal WebServerConfigurationBuilder()
    {
    }
}