namespace Sentinel.MCP.Contracts;

public interface IToolResult<T>
{
    bool Success { get; }
    T? Data { get; }

#pragma warning disable CA1716
    ToolError? Error { get; }
#pragma warning restore CA1716

    DateTime ExecutedAtUtc { get; }
    long DurationMs { get; }
}
