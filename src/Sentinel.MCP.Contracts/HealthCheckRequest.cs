using System.Text.Json.Serialization;

namespace Sentinel.MCP.Contracts;

public sealed class HealthCheckRequest
{
#pragma warning disable CA1056
    [JsonPropertyName("url")]
    public required string Url { get; init; }
#pragma warning restore CA1056

    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; init; } = 10;

    [JsonPropertyName("followRedirects")]
    public bool FollowRedirects { get; init; } = true;
}
