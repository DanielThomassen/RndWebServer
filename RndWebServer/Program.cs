using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RndWebServer;
using RndWebServer.Server;
using RndWebServer.Server.Internal;

var builder = new WebServerBuilder();

builder.Services.AddLogging(opt =>
{
    opt.SetMinimumLevel(LogLevel.Debug);
    opt.AddConsole();
});
builder.Services.AddRouting();

var server = builder.Build();

server.Configure(options =>
{
    options.AddRoute<IndexRoute>("");
});

await server.StartAsync(5001, default);