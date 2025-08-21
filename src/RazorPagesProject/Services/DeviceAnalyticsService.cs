namespace RazorPagesProject.Services;

/// <summary>
/// Service to analyze device usage analytics and determine mobile usage percentage
/// </summary>
public interface IDeviceAnalyticsService
{
    /// <summary>
    /// Calculate mobile usage percentage from logs
    /// </summary>
    Task<double> GetMobileUsagePercentageAsync();
    
    /// <summary>
    /// Determine if hamburger menu should be enabled based on mobile usage threshold
    /// </summary>
    Task<bool> ShouldEnableHamburgerMenuAsync();
}

public class DeviceAnalyticsService : IDeviceAnalyticsService
{
    private readonly ILogger<DeviceAnalyticsService> _logger;
    private readonly IConfiguration _configuration;

    public DeviceAnalyticsService(ILogger<DeviceAnalyticsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<double> GetMobileUsagePercentageAsync()
    {
        // For demo purposes, we'll simulate analytics data
        // In a real implementation, this would read from log files or a logging aggregation service
        await Task.Delay(1); // Simulate async operation
        
        // Simulate some sample data for demo
        // This would typically read from structured logs, database, or log aggregation service
        var sampleData = GetSimulatedAnalyticsData();
        
        var totalSessions = sampleData.Count;
        var mobileSessions = sampleData.Count(d => d.DeviceType == "Mobile");
        
        if (totalSessions == 0)
        {
            _logger.LogWarning("No analytics data available, defaulting to 0% mobile usage");
            return 0.0;
        }
        
        var mobilePercentage = (double)mobileSessions / totalSessions * 100;
        
        _logger.LogInformation("Analytics: Total={Total}, Mobile={Mobile}, Percentage={Percentage:F1}%", 
            totalSessions, mobileSessions, mobilePercentage);
            
        return mobilePercentage;
    }

    public async Task<bool> ShouldEnableHamburgerMenuAsync()
    {
        var threshold = _configuration.GetValue<double>("Navigation:MobileUsageThreshold", 30.0);
        var mobileUsage = await GetMobileUsagePercentageAsync();
        
        var shouldEnable = mobileUsage >= threshold;
        
        _logger.LogInformation("Hamburger menu decision: Mobile usage {Usage:F1}% vs threshold {Threshold}% = {Decision}",
            mobileUsage, threshold, shouldEnable ? "ENABLED" : "DISABLED");
            
        return shouldEnable;
    }

    private List<DeviceAnalyticsData> GetSimulatedAnalyticsData()
    {
        // Simulated 1-week sample data for demo purposes
        // In production, this would come from actual log analysis
        return new List<DeviceAnalyticsData>
        {
            // Simulate higher mobile usage (>30%) for testing
            new("Mobile", DateTime.Now.AddDays(-1)),
            new("Mobile", DateTime.Now.AddDays(-1)),
            new("Mobile", DateTime.Now.AddDays(-2)),
            new("Desktop", DateTime.Now.AddDays(-2)),
            new("Mobile", DateTime.Now.AddDays(-3)),
            new("Tablet", DateTime.Now.AddDays(-3)),
            new("Mobile", DateTime.Now.AddDays(-4)),
            new("Desktop", DateTime.Now.AddDays(-4)),
            new("Mobile", DateTime.Now.AddDays(-5)),
            new("Mobile", DateTime.Now.AddDays(-6)),
            new("Desktop", DateTime.Now.AddDays(-6)),
            new("Mobile", DateTime.Now.AddDays(-7))
        };
    }
}

public record DeviceAnalyticsData(string DeviceType, DateTime Timestamp);