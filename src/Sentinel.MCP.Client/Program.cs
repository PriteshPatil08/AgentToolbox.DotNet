using Microsoft.Extensions.Hosting;

// Block 1: Configuration
var builder = Host.CreateApplicationBuilder(args);
var apiKey = builder.Configuration["Anthropic:ApiKey"]
    ?? throw new InvalidOperationException("Anthropic:ApiKey not set in user-secrets.");

// Block 2: MCP client — spawn server + connect
using ModelContextProtocol.Client;

await using var mcpClient = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Command = "dotnet",
        Arguments = ["run", "--project", "src/Sentinel.MCP.Server", "--no-launch-profile"],
        Name = "Sentinel"
    }));

var tools = await mcpClient.GetAIFunctionsAsync();

// Block 3: Claude IChatClient with tool invocation middleware

// Block 4: Chat loop
