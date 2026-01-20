using RazorPagesProject.Services;

namespace RazorPagesProject.Middleware;

public class DeviceAnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DeviceAnalyticsMiddleware> _logger;

    public DeviceAnalyticsMiddleware(RequestDelegate next, ILogger<DeviceAnalyticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IDeviceAnalyticsService analyticsService)
    {
        if (context.Request.Path.StartsWithSegments("/Analytics", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var userAgent = context.Request.Headers.UserAgent.ToString();
        
        if (!string.IsNullOrEmpty(userAgent))
        {
            try
            {
                analyticsService.RecordRequest(userAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording device analytics");
            }
        }

        await _next(context);
    }
}
