using FluentValidation;

namespace Sentinel.MCP.Contracts;

public sealed class HealthCheckRequestValidator : AbstractValidator<HealthCheckRequest>
{
    public HealthCheckRequestValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required.")
            .Must(BeAValidAbsoluteUri)
            .WithMessage("Must be a valid absolute URI with http or https scheme. Example: https://api.github.com");

        RuleFor(x => x.TimeoutSeconds)
            .InclusiveBetween(1, 60)
            .WithMessage("TimeoutSeconds must be between 1 and 60.");
    }

    private static bool BeAValidAbsoluteUri(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }
}
