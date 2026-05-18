using System.Text.Json.Serialization;

namespace Sentinel.MCP.Contracts;

public sealed class SSLCertificateResponse
{
    [JsonPropertyName("subject")]
    public required string Subject { get; init; }

    [JsonPropertyName("issuer")]
    public required string Issuer { get; init; }

    [JsonPropertyName("validFrom")]
    public DateTime ValidFrom { get; init; }

    [JsonPropertyName("expiresOn")]
    public DateTime ExpiresOn { get; init; }

    [JsonPropertyName("daysUntilExpiry")]
    public int DaysUntilExpiry { get; init; }

    [JsonPropertyName("isExpired")]
    public bool IsExpired { get; init; }

    [JsonPropertyName("isExpiringSoon")]
    public bool IsExpiringSoon { get; init; }

    [JsonPropertyName("tlsVersion")]
    public required string TlsVersion { get; init; }

    [JsonPropertyName("certificateChainValid")]
    public bool CertificateChainValid { get; init; }

#pragma warning disable CA1002
    [JsonPropertyName("subjectAlternativeNames")]
    public List<string> SubjectAlternativeNames { get; init; } = [];
#pragma warning restore CA1002

    [JsonPropertyName("thumbprint")]
    public required string Thumbprint { get; init; }
}
