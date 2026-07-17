namespace RazorPagesProject.Models;

public class DeviceAnalytics
{
    public DateTime Timestamp { get; set; }
    public DeviceType DeviceType { get; set; }
    public string UserAgent { get; set; } = string.Empty;
}

public enum DeviceType
{
    Desktop,
    Mobile,
    Tablet,
    Unknown
}

public class DeviceAnalyticsSummary
{
    public int TotalRequests { get; set; }
    public int MobileRequests { get; set; }
    public int TabletRequests { get; set; }
    public int DesktopRequests { get; set; }
    public int UnknownRequests { get; set; }
    public double MobilePercentage { get; set; }
    public double TabletPercentage { get; set; }
    public double DesktopPercentage { get; set; }
    public DateTime FirstRequest { get; set; }
    public DateTime LastRequest { get; set; }
}
