using System.Text.Json.Serialization;

namespace Sentinel.MCP.Contracts;

public sealed class ToolError
{
    [JsonPropertyName("errorCode")]
    public ToolErrorCode ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("fieldErrors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? FieldErrors { get; init; }

    [JsonPropertyName("retryAfterSeconds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? RetryAfterSeconds { get; init; }
}
