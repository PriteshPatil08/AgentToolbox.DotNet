using Anthropic;
using Anthropic.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>(optional: true);
var apiKey = builder.Configuration["Anthropic:ApiKey"]
    ?? throw new InvalidOperationException("Anthropic:ApiKey not set in user-secrets.");

#pragma warning disable CA2007
await using var mcpClient = await McpClient.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Command = "dotnet",
        Arguments = ["run", "--project", "src/Sentinel.MCP.Server", "--no-launch-profile"],
        WorkingDirectory = Directory.GetCurrentDirectory(),
        Name = "Sentinel"
    })).ConfigureAwait(false);
#pragma warning restore CA2007

var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);

Environment.SetEnvironmentVariable("ANTHROPIC_API_KEY", apiKey);

#pragma warning disable CA2000 // AnthropicClient does not implement IDisposable
AnthropicClient anthropicClient = new();
#pragma warning restore CA2000

using IChatClient chatClient = anthropicClient
    .AsIChatClient("claude-sonnet-4-5-20250929")
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var history = new List<ChatMessage>();

Console.WriteLine("Sentinel is ready. Ask anything (or type 'exit' to quit).");
Console.WriteLine();

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    history.Add(new ChatMessage(ChatRole.User, input));

    var response = await chatClient.GetResponseAsync(
        history,
        new ChatOptions
        {
            MaxOutputTokens = 4096,
            Tools = [.. tools]
        }).ConfigureAwait(false);

    history.AddRange(response.Messages);

    Console.WriteLine();
    Console.WriteLine(response.Text ?? "(no response)");
    Console.WriteLine();
}
