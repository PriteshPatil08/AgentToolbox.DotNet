namespace Sentinel.MCP.Contracts;

public sealed class HealthCheckRequest
{
    public Uri? Url { get; init; }
    public int TimeoutSeconds { get; init; } = 10;
    public bool FollowRedirects { get; init; } = true;
}
