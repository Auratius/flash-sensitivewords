using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Flash.SensitiveWords.API.Middleware;

/// <summary>
/// Lightweight middleware for tracking API request performance metrics
/// </summary>
public class PerformanceMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMetricsMiddleware> _logger;

    public PerformanceMetricsMiddleware(RequestDelegate next, ILogger<PerformanceMetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMs = sw.ElapsedMilliseconds;

            // Log with structured data for easy querying
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                requestMethod,
                requestPath,
                statusCode,
                elapsedMs
            );

            // Track slow requests
            if (elapsedMs > 1000) // More than 1 second
            {
                _logger.LogWarning(
                    "SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms (Status: {StatusCode})",
                    requestMethod,
                    requestPath,
                    elapsedMs,
                    statusCode
                );
            }
        }
    }
}
