
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RndWebServer.Server;

var builder = new WebServerBuilder();

builder.AddMiddleware<HelloMiddleWare>();

builder.Services.AddLogging(opt =>
{
    opt.AddConsole();
});

await builder.Build().StartAsync(5001, default);