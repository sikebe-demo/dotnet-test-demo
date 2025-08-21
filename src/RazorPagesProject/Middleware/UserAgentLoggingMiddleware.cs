using UAParser;

namespace RazorPagesProject.Middleware;

/// <summary>
/// Middleware to log User-Agent information for device type analytics
/// </summary>
public class UserAgentLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserAgentLoggingMiddleware> _logger;

    public UserAgentLoggingMiddleware(RequestDelegate next, ILogger<UserAgentLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only log for page requests (not static assets)
        if (IsPageRequest(context.Request.Path))
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                var deviceType = GetDeviceType(userAgent);
                
                // Log device type for analytics (structured logging)
                _logger.LogInformation("UserAgent analytics: DeviceType={DeviceType}, UserAgent={UserAgent}, IP={IP}, Path={Path}",
                    deviceType, userAgent, GetClientIpAddress(context), context.Request.Path);
            }
        }

        await _next(context);
    }

    private static string GetDeviceType(string userAgent)
    {
        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(userAgent);
        
        // Simple device categorization based on User-Agent parsing
        var deviceFamily = clientInfo.Device.Family?.ToLowerInvariant() ?? "";
        var osFamily = clientInfo.OS.Family?.ToLowerInvariant() ?? "";
        
        // Mobile devices
        if (deviceFamily.Contains("iphone") || deviceFamily.Contains("android") || 
            deviceFamily.Contains("mobile") || osFamily.Contains("ios") ||
            osFamily.Contains("android") || userAgent.ToLowerInvariant().Contains("mobile"))
        {
            return "Mobile";
        }
        
        // Tablet devices  
        if (deviceFamily.Contains("ipad") || deviceFamily.Contains("tablet") ||
            userAgent.ToLowerInvariant().Contains("tablet"))
        {
            return "Tablet";
        }
        
        // Default to Desktop
        return "Desktop";
    }

    private static bool IsPageRequest(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? "";
        
        // Skip static assets and API endpoints
        return !pathValue.StartsWith("/lib/") &&
               !pathValue.StartsWith("/css/") &&
               !pathValue.StartsWith("/js/") &&
               !pathValue.StartsWith("/images/") &&
               !pathValue.StartsWith("/api/") &&
               !pathValue.Contains("favicon") &&
               !path.HasValue || 
               pathValue.EndsWith("/") ||
               !Path.HasExtension(pathValue);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Handle various proxy scenarios
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }
        return ip ?? "unknown";
    }
}