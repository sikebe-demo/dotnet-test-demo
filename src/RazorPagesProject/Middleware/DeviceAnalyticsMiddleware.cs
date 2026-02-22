namespace RazorPagesProject.Middleware;

public class DeviceAnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DeviceAnalyticsMiddleware> _logger;
    private static readonly object _lock = new();
    private static DeviceStatistics _statistics = new();

    public DeviceAnalyticsMiddleware(RequestDelegate next, ILogger<DeviceAnalyticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var deviceType = ClassifyDevice(userAgent);

        lock (_lock)
        {
            _statistics.TotalRequests++;
            switch (deviceType)
            {
                case DeviceType.Mobile:
                    _statistics.MobileRequests++;
                    break;
                case DeviceType.Tablet:
                    _statistics.TabletRequests++;
                    break;
                case DeviceType.Desktop:
                    _statistics.DesktopRequests++;
                    break;
            }
        }

        await _next(context);
    }

    private static DeviceType ClassifyDevice(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return DeviceType.Desktop;
        }

        var ua = userAgent.ToLowerInvariant();

        if (ua.Contains("ipad") || ua.Contains("tablet") || ua.Contains("kindle"))
        {
            return DeviceType.Tablet;
        }

        if (ua.Contains("android"))
        {
            if (ua.Contains("mobile"))
            {
                return DeviceType.Mobile;
            }
            return DeviceType.Tablet;
        }

        if (ua.Contains("mobile") || ua.Contains("iphone") || ua.Contains("ipod"))
        {
            return DeviceType.Mobile;
        }

        return DeviceType.Desktop;
    }

    public static DeviceStatistics GetStatistics()
    {
        lock (_lock)
        {
            return new DeviceStatistics
            {
                TotalRequests = _statistics.TotalRequests,
                MobileRequests = _statistics.MobileRequests,
                TabletRequests = _statistics.TabletRequests,
                DesktopRequests = _statistics.DesktopRequests
            };
        }
    }

    public static void ResetStatistics()
    {
        lock (_lock)
        {
            _statistics = new DeviceStatistics();
        }
    }
}

public enum DeviceType
{
    Desktop,
    Mobile,
    Tablet
}

public class DeviceStatistics
{
    public long TotalRequests { get; set; }
    public long MobileRequests { get; set; }
    public long TabletRequests { get; set; }
    public long DesktopRequests { get; set; }

    public double MobilePercentage => TotalRequests > 0 ? (MobileRequests * 100.0 / TotalRequests) : 0;
    public double TabletPercentage => TotalRequests > 0 ? (TabletRequests * 100.0 / TotalRequests) : 0;
    public double DesktopPercentage => TotalRequests > 0 ? (DesktopRequests * 100.0 / TotalRequests) : 0;
}
