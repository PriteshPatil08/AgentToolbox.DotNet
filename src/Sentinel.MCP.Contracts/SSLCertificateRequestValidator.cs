using FluentValidation;

namespace Sentinel.MCP.Contracts;

public sealed class SSLCertificateRequestValidator : AbstractValidator<SSLCertificateRequest>
{
    public SSLCertificateRequestValidator()
    {
        RuleFor(x => x.Hostname)
            .NotEmpty()
            .WithMessage("Hostname is required.")
            .Must(NotContainScheme)
            .WithMessage("Hostname must not include a protocol scheme (http:// or https://). Provide just the domain, e.g. api.github.com")
            .Must(NotContainPath)
            .WithMessage("Hostname must not include a path. Provide just the domain, e.g. api.github.com");

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535)
            .WithMessage("Port must be between 1 and 65535.");
    }

    private static bool NotContainScheme(string? hostname) =>
        !string.IsNullOrWhiteSpace(hostname) && !hostname.Contains("://", StringComparison.OrdinalIgnoreCase);

    private static bool NotContainPath(string? hostname) =>
        !string.IsNullOrWhiteSpace(hostname) && !hostname.Contains('/', StringComparison.OrdinalIgnoreCase);
}
