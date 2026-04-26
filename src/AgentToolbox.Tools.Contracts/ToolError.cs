namespace AgentToolbox.Tools.Contracts;

public sealed class ToolError
{
    public ToolErrorCode ErrorCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public Dictionary<string, string[]>? FieldErrors { get; init; }
}
