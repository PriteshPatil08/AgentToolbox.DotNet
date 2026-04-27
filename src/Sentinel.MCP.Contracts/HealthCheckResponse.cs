namespace Sentinel.MCP.Contracts;

public sealed class HealthCheckResponse
{
    public int StatusCode { get; init; }
    public string StatusDescription { get; init; } = string.Empty;
    public long LatencyMs { get; init; }
    public string? ContentType { get; init; }
    public Dictionary<string, string> ResponseHeaders { get; init; } = [];
    public bool IsHealthy { get; init; }
    public string? ServerHeader { get; init; }
}
