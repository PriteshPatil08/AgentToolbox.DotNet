using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
    options.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport();

builder.Services.Configure<McpServerOptions>(options =>
{
    options.ServerInfo = new Implementation
    {
        Name = builder.Configuration["McpServer:Name"] ?? "AgentToolbox",
        Version = builder.Configuration["McpServer:Version"] ?? "1.0.0"
    };
});

await builder.Build().RunAsync().ConfigureAwait(false);
