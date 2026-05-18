using System.Text.Json.Serialization;

namespace Sentinel.MCP.Contracts;

public sealed class SSLCertificateRequest
{
    [JsonPropertyName("hostname")]
    public required string Hostname { get; init; }

    [JsonPropertyName("port")]
    public int Port { get; init; } = 443;
}
