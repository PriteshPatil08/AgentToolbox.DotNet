using System.Text.Json.Serialization;

namespace Sentinel.MCP.Contracts;

public sealed class HealthCheckResponse
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    [JsonPropertyName("statusDescription")]
    public string StatusDescription { get; init; } = string.Empty;

    [JsonPropertyName("latencyMs")]
    public long LatencyMs { get; init; }

    [JsonPropertyName("contentType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentType { get; init; }

    [JsonPropertyName("responseHeaders")]
    public Dictionary<string, string> ResponseHeaders { get; init; } = [];

    [JsonPropertyName("isHealthy")]
    public bool IsHealthy { get; init; }

    [JsonPropertyName("serverHeader")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ServerHeader { get; init; }
}
