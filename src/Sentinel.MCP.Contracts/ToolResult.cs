using System.Text.Json.Serialization;

namespace Sentinel.MCP.Contracts;

public sealed class ToolResult<T> : IToolResult<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToolError? Error { get; init; }

    [JsonPropertyName("executedAtUtc")]
    public DateTime ExecutedAtUtc { get; init; }

    [JsonPropertyName("durationMs")]
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
        Error = error,
        ExecutedAtUtc = DateTime.UtcNow,
        DurationMs = durationMs
    };
}
