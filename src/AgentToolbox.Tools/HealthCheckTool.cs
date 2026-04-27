using System.ComponentModel;
using System.Diagnostics;
using AgentToolbox.Tools.Contracts;
using ModelContextProtocol.Server;

namespace AgentToolbox.Tools;

[McpServerToolType]
public sealed class HealthCheckTool
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HealthCheckTool(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [McpServerTool(Name = "HealthCheck")]
    [Description("Performs an HTTP health check against a given URL. Returns status code, latency, response headers, and a health assessment. Use this when asked about whether an API or website is up, slow, or responding correctly.")]
    public async Task<ToolResult<HealthCheckResponse>> CheckAsync(
        [Description("The URL to check")] Uri url,
        [Description("Timeout in seconds (1-60)")] int timeoutSeconds = 10,
        [Description("Whether to follow HTTP redirects")] bool followRedirects = true,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var clientName = followRedirects ? "HealthCheck.Follow" : "HealthCheck.NoFollow";
            var client = _httpClientFactory.CreateClient(clientName);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            using var response = await client.GetAsync(url, cts.Token).ConfigureAwait(false);
            stopwatch.Stop();

            var headers = response.Headers
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

            return ToolResult.Ok(new HealthCheckResponse
            {
                StatusCode = (int)response.StatusCode,
                StatusDescription = response.ReasonPhrase ?? response.StatusCode.ToString(),
                LatencyMs = stopwatch.ElapsedMilliseconds,
                ContentType = response.Content.Headers.ContentType?.ToString(),
                ResponseHeaders = headers,
                IsHealthy = (int)response.StatusCode is >= 200 and < 300,
                ServerHeader = response.Headers.Server?.ToString()
            }, stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            return ToolResult.Fail<HealthCheckResponse>(new ToolError
            {
                ErrorCode = ToolErrorCode.Timeout,
                Message = $"Request timed out after {timeoutSeconds}s"
            }, stopwatch.ElapsedMilliseconds);
        }
        catch (HttpRequestException ex) when (ex.HttpRequestError == HttpRequestError.NameResolutionError)
        {
            stopwatch.Stop();
            return ToolResult.Fail<HealthCheckResponse>(new ToolError
            {
                ErrorCode = ToolErrorCode.ConnectionFailed,
                Message = $"DNS resolution failed: {ex.Message}"
            }, stopwatch.ElapsedMilliseconds);
        }
        catch (HttpRequestException ex) when (ex.HttpRequestError == HttpRequestError.SecureConnectionError)
        {
            stopwatch.Stop();
            return ToolResult.Fail<HealthCheckResponse>(new ToolError
            {
                ErrorCode = ToolErrorCode.SslError,
                Message = $"SSL/TLS error: {ex.Message}"
            }, stopwatch.ElapsedMilliseconds);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            return ToolResult.Fail<HealthCheckResponse>(new ToolError
            {
                ErrorCode = ToolErrorCode.ConnectionFailed,
                Message = ex.Message
            }, stopwatch.ElapsedMilliseconds);
        }
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            stopwatch.Stop();
            return ToolResult.Fail<HealthCheckResponse>(new ToolError
            {
                ErrorCode = ToolErrorCode.Unknown,
                Message = ex.Message
            }, stopwatch.ElapsedMilliseconds);
        }
    }
}
