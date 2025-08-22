using UAParser;

namespace RazorPagesProject.Middleware;

/// <summary>
/// Middleware for collecting anonymized access analytics to analyze device types and make data-driven decisions
/// </summary>
public class AccessAnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AccessAnalyticsMiddleware> _logger;
    private static readonly Dictionary<string, AccessMetrics> _analytics = new();
    private static readonly object _lockObject = new();

    public AccessAnalyticsMiddleware(RequestDelegate next, ILogger<AccessAnalyticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only track page requests, not static resources
        if (IsPageRequest(context.Request.Path))
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var deviceInfo = AnalyzeUserAgent(userAgent);
            
            RecordAccess(deviceInfo);
            
            // Log for debugging in development
            if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                _logger.LogInformation("Access logged: {DeviceType} from {UserAgent}", 
                    deviceInfo.DeviceType, userAgent);
            }
        }

        await _next(context);
    }

    private static bool IsPageRequest(PathString path)
    {
        // Track actual page requests, exclude static resources
        var pathValue = path.Value?.ToLowerInvariant() ?? "";
        return !pathValue.Contains("/css/") && 
               !pathValue.Contains("/js/") && 
               !pathValue.Contains("/lib/") && 
               !pathValue.Contains("/images/") &&
               !pathValue.Contains(".ico") &&
               !pathValue.Contains(".png") &&
               !pathValue.Contains(".jpg") &&
               !pathValue.Contains(".svg");
    }

    private static DeviceInfo AnalyzeUserAgent(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return new DeviceInfo { DeviceType = "Unknown", UserAgent = "Unknown" };
        }

        var parser = Parser.GetDefault();
        var clientInfo = parser.Parse(userAgent);
        
        var deviceType = DetermineDeviceType(clientInfo, userAgent);
        
        return new DeviceInfo
        {
            DeviceType = deviceType,
            Browser = clientInfo.UA.Family,
            OS = clientInfo.OS.Family,
            UserAgent = userAgent
        };
    }

    private static string DetermineDeviceType(ClientInfo clientInfo, string userAgent)
    {
        var ua = userAgent.ToLowerInvariant();
        
        // Mobile device detection
        if (ua.Contains("mobile") || 
            ua.Contains("android") || 
            ua.Contains("iphone") || 
            ua.Contains("ipod") ||
            ua.Contains("blackberry") ||
            ua.Contains("windows phone"))
        {
            return "Mobile";
        }
        
        // Tablet detection
        if (ua.Contains("tablet") || 
            ua.Contains("ipad") ||
            (ua.Contains("android") && !ua.Contains("mobile")))
        {
            return "Tablet";
        }
        
        return "Desktop";
    }

    private static void RecordAccess(DeviceInfo deviceInfo)
    {
        lock (_lockObject)
        {
            var today = DateTime.UtcNow.Date;
            var key = $"{today:yyyy-MM-dd}_{deviceInfo.DeviceType}";
            
            if (_analytics.ContainsKey(key))
            {
                _analytics[key].Count++;
                _analytics[key].LastAccess = DateTime.UtcNow;
            }
            else
            {
                _analytics[key] = new AccessMetrics
                {
                    Date = today,
                    DeviceType = deviceInfo.DeviceType,
                    Count = 1,
                    FirstAccess = DateTime.UtcNow,
                    LastAccess = DateTime.UtcNow
                };
            }
        }
    }

    /// <summary>
    /// Get current analytics data (for admin/development purposes)
    /// </summary>
    public static Dictionary<string, AccessMetrics> GetAnalytics()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, AccessMetrics>(_analytics);
        }
    }

    /// <summary>
    /// Get mobile usage percentage for decision making
    /// </summary>
    public static double GetMobileUsagePercentage()
    {
        lock (_lockObject)
        {
            var today = DateTime.UtcNow.Date;
            var recentDays = 7; // Look at last 7 days
            
            long totalCount = 0;
            long mobileCount = 0;
            
            for (int i = 0; i < recentDays; i++)
            {
                var date = today.AddDays(-i);
                var dateKey = date.ToString("yyyy-MM-dd");
                
                foreach (var kvp in _analytics.Where(a => a.Key.StartsWith(dateKey)))
                {
                    totalCount += kvp.Value.Count;
                    if (kvp.Value.DeviceType == "Mobile")
                    {
                        mobileCount += kvp.Value.Count;
                    }
                }
            }
            
            return totalCount > 0 ? (double)mobileCount / totalCount * 100 : 0;
        }
    }
}

public class DeviceInfo
{
    public string DeviceType { get; set; } = "";
    public string Browser { get; set; } = "";
    public string OS { get; set; } = "";
    public string UserAgent { get; set; } = "";
}

public class AccessMetrics
{
    public DateTime Date { get; set; }
    public string DeviceType { get; set; } = "";
    public long Count { get; set; }
    public DateTime FirstAccess { get; set; }
    public DateTime LastAccess { get; set; }
}