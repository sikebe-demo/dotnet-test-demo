using RazorPagesProject.Middleware;
using Xunit;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

[Collection("DeviceAnalytics")]
public class DeviceAnalyticsTests
{
    public DeviceAnalyticsTests()
    {
        DeviceAnalyticsMiddleware.ResetStatistics();
    }

    [Fact]
    public void GetStatistics_ReturnsCorrectInitialState()
    {
        var stats = DeviceAnalyticsMiddleware.GetStatistics();
        
        Assert.Equal(0, stats.TotalRequests);
        Assert.Equal(0, stats.MobileRequests);
        Assert.Equal(0, stats.TabletRequests);
        Assert.Equal(0, stats.DesktopRequests);
        Assert.Equal(0, stats.MobilePercentage);
        Assert.Equal(0, stats.TabletPercentage);
        Assert.Equal(0, stats.DesktopPercentage);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1", "Mobile")]
    [InlineData("Mozilla/5.0 (Linux; Android 10; SM-G973F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.120 Mobile Safari/537.36", "Mobile")]
    [InlineData("Mozilla/5.0 (iPad; CPU OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1", "Tablet")]
    [InlineData("Mozilla/5.0 (Linux; Android 9; SM-T830) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.120 Safari/537.36", "Tablet")]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36", "Desktop")]
    [InlineData("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36", "Desktop")]
    public async Task Middleware_ClassifiesUserAgentCorrectly(string userAgent, string expectedDeviceType)
    {
        DeviceAnalyticsMiddleware.ResetStatistics();
        
        var context = new DefaultHttpContext();
        context.Request.Headers.UserAgent = userAgent;
        
        var middleware = new DeviceAnalyticsMiddleware(
            next: (innerHttpContext) => Task.CompletedTask,
            logger: new Microsoft.Extensions.Logging.Abstractions.NullLogger<DeviceAnalyticsMiddleware>()
        );

        await middleware.InvokeAsync(context);

        var stats = DeviceAnalyticsMiddleware.GetStatistics();
        Assert.Equal(1, stats.TotalRequests);

        switch (expectedDeviceType)
        {
            case "Mobile":
                Assert.Equal(1, stats.MobileRequests);
                Assert.Equal(0, stats.TabletRequests);
                Assert.Equal(0, stats.DesktopRequests);
                break;
            case "Tablet":
                Assert.Equal(0, stats.MobileRequests);
                Assert.Equal(1, stats.TabletRequests);
                Assert.Equal(0, stats.DesktopRequests);
                break;
            case "Desktop":
                Assert.Equal(0, stats.MobileRequests);
                Assert.Equal(0, stats.TabletRequests);
                Assert.Equal(1, stats.DesktopRequests);
                break;
        }
    }

    [Fact]
    public void Statistics_CalculatesPercentagesCorrectly()
    {
        var stats = new DeviceStatistics
        {
            TotalRequests = 100,
            MobileRequests = 30,
            TabletRequests = 20,
            DesktopRequests = 50
        };

        Assert.Equal(30.0, stats.MobilePercentage);
        Assert.Equal(20.0, stats.TabletPercentage);
        Assert.Equal(50.0, stats.DesktopPercentage);
    }

    [Fact]
    public void Statistics_HandlesZeroRequestsGracefully()
    {
        var stats = new DeviceStatistics
        {
            TotalRequests = 0,
            MobileRequests = 0,
            TabletRequests = 0,
            DesktopRequests = 0
        };

        Assert.Equal(0, stats.MobilePercentage);
        Assert.Equal(0, stats.TabletPercentage);
        Assert.Equal(0, stats.DesktopPercentage);
    }
}

[CollectionDefinition("DeviceAnalytics", DisableParallelization = true)]
public class DeviceAnalyticsCollection
{
}
