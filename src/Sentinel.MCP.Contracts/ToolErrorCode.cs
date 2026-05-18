namespace Sentinel.MCP.Contracts;

public enum ToolErrorCode
{
    None                = 0,
    ValidationFailed    = 100,
    Timeout             = 200,
    DnsResolutionFailed = 301,
    ConnectionRefused   = 302,
    ConnectionFailed    = 303,
    SSLError            = 400,
    RateLimited         = 429,
    InsufficientData    = 500,
    Unknown             = 999
}
