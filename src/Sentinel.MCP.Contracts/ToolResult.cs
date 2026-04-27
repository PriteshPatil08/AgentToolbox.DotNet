namespace Sentinel.MCP.Contracts;

public sealed class ToolResult<T> : IToolResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public ToolError? Failure { get; init; }
    public DateTime ExecutedAtUtc { get; init; }
    public long DurationMs { get; init; }
}

public static class ToolResult
{
    public static ToolResult<T> Ok<T>(T data, long durationMs) => new()
    {
        Success = true,
        Data = data,
        ExecutedAtUtc = DateTime.UtcNow,
        DurationMs = durationMs
    };

    public static ToolResult<T> Fail<T>(ToolError error, long durationMs) => new()
    {
        Success = false,
        Failure = error,
        ExecutedAtUtc = DateTime.UtcNow,
        DurationMs = durationMs
    };
}
