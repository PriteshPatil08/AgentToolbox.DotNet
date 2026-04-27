namespace Sentinel.MCP.Contracts;

public interface IToolResult<out T>
{
    bool Success { get; }
    T? Data { get; }
    ToolError? Failure { get; }
    DateTime ExecutedAtUtc { get; }
    long DurationMs { get; }
}
