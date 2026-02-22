using RazorPagesProject.Models;
using System.Collections.Concurrent;

namespace RazorPagesProject.Services;

public class DeviceAnalyticsService : IDeviceAnalyticsService
{
    private const int MaxUserAgentLength = 200;
    
    private readonly ConcurrentBag<DeviceAnalytics> _analytics = new();
    private readonly ILogger<DeviceAnalyticsService> _logger;

    public DeviceAnalyticsService(ILogger<DeviceAnalyticsService> logger)
    {
        _logger = logger;
    }

    public void RecordRequest(string userAgent)
    {
        var deviceType = ClassifyDevice(userAgent);
        var analytics = new DeviceAnalytics
        {
            Timestamp = DateTime.UtcNow,
            DeviceType = deviceType,
            UserAgent = SanitizeUserAgent(userAgent)
        };

        _analytics.Add(analytics);
        _logger.LogDebug("Recorded {DeviceType} request", deviceType);
    }

    public DeviceAnalyticsSummary GetSummary()
    {
        var records = _analytics.ToArray();
        
        if (records.Length == 0)
        {
            return new DeviceAnalyticsSummary
            {
                FirstRequest = DateTime.UtcNow,
                LastRequest = DateTime.UtcNow
            };
        }

        var deviceCounts = records.GroupBy(r => r.DeviceType)
            .ToDictionary(g => g.Key, g => g.Count());

        var mobileCount = deviceCounts.GetValueOrDefault(DeviceType.Mobile, 0);
        var tabletCount = deviceCounts.GetValueOrDefault(DeviceType.Tablet, 0);
        var desktopCount = deviceCounts.GetValueOrDefault(DeviceType.Desktop, 0);
        var unknownCount = deviceCounts.GetValueOrDefault(DeviceType.Unknown, 0);
        var total = records.Length;

        return new DeviceAnalyticsSummary
        {
            TotalRequests = total,
            MobileRequests = mobileCount,
            TabletRequests = tabletCount,
            DesktopRequests = desktopCount,
            UnknownRequests = unknownCount,
            MobilePercentage = total > 0 ? (double)mobileCount / total * 100 : 0,
            TabletPercentage = total > 0 ? (double)tabletCount / total * 100 : 0,
            DesktopPercentage = total > 0 ? (double)desktopCount / total * 100 : 0,
            FirstRequest = records.Min(r => r.Timestamp),
            LastRequest = records.Max(r => r.Timestamp)
        };
    }

    public DeviceType ClassifyDevice(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return DeviceType.Unknown;
        }

        var ua = userAgent.ToLowerInvariant();

        if (IsMobile(ua))
        {
            return DeviceType.Mobile;
        }

        if (IsTablet(ua))
        {
            return DeviceType.Tablet;
        }

        if (IsDesktop(ua))
        {
            return DeviceType.Desktop;
        }

        return DeviceType.Unknown;
    }

    private static bool IsMobile(string userAgent)
    {
        var mobileKeywords = new[]
        {
            "mobile", "android", "iphone", "ipod", "windows phone",
            "blackberry", "opera mini", "iemobile", "mobile safari"
        };

        var tabletKeywords = new[] { "ipad", "tablet", "kindle" };

        return mobileKeywords.Any(keyword => userAgent.Contains(keyword)) &&
               !tabletKeywords.Any(keyword => userAgent.Contains(keyword));
    }

    private static bool IsTablet(string userAgent)
    {
        var tabletKeywords = new[]
        {
            "ipad", "tablet", "kindle", "silk", "playbook", "nexus 7",
            "nexus 10", "xoom", "sch-i800", "nexus 9"
        };

        return tabletKeywords.Any(keyword => userAgent.Contains(keyword));
    }

    private static bool IsDesktop(string userAgent)
    {
        var desktopKeywords = new[]
        {
            "windows nt", "macintosh", "linux", "x11"
        };

        return desktopKeywords.Any(keyword => userAgent.Contains(keyword));
    }

    private static string SanitizeUserAgent(string userAgent)
    {
        return string.IsNullOrEmpty(userAgent) ? "Unknown" : userAgent[..Math.Min(MaxUserAgentLength, userAgent.Length)];
    }
}
