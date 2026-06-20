using Microsoft.AspNetCore.Mvc.Testing;
using RazorPagesProject.Models;
using RazorPagesProject.Services;
using Microsoft.Extensions.DependencyInjection;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class AnalyticsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AnalyticsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_AnalyticsPage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Analytics");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8",
            response?.Content?.Headers?.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_AnalyticsPage_ContainsExpectedContent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Analytics");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Contains("Device Access Analytics", content);
        Assert.Contains("モバイル・タブレット・デスクトップのアクセス分析", content);
    }

    [Fact]
    public async Task DeviceAnalyticsMiddleware_RecordsRequests()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

        // Act - Make multiple requests to generate analytics data
        await client.GetAsync("/");
        await client.GetAsync("/About");
        
        // Verify analytics page shows data
        var analyticsResponse = await client.GetAsync("/Analytics");
        var content = await analyticsResponse.Content.ReadAsStringAsync();

        // Assert - Check that the analytics page shows collected data
        analyticsResponse.EnsureSuccessStatusCode();
        Assert.Contains("総リクエスト数", content);
        Assert.DoesNotContain("データ収集中", content);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)", DeviceType.Mobile)]
    [InlineData("Mozilla/5.0 (iPad; CPU OS 14_0 like Mac OS X)", DeviceType.Tablet)]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64)", DeviceType.Desktop)]
    [InlineData("", DeviceType.Unknown)]
    public void DeviceAnalyticsService_ClassifiesDeviceCorrectly(string userAgent, DeviceType expectedType)
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var analyticsService = scope.ServiceProvider.GetRequiredService<IDeviceAnalyticsService>();

        // Act
        var deviceType = analyticsService.ClassifyDevice(userAgent);

        // Assert
        Assert.Equal(expectedType, deviceType);
    }

    [Fact]
    public void DeviceAnalyticsService_CalculatesPercentagesCorrectly()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var analyticsService = scope.ServiceProvider.GetRequiredService<IDeviceAnalyticsService>();

        // Act - Record different device types
        analyticsService.RecordRequest("Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)");
        analyticsService.RecordRequest("Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X)");
        analyticsService.RecordRequest("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        analyticsService.RecordRequest("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)");
        
        var summary = analyticsService.GetSummary();

        // Assert
        Assert.True(summary.TotalRequests >= 4, $"Expected at least 4 requests, got {summary.TotalRequests}");
        Assert.True(summary.MobileRequests >= 2, $"Expected at least 2 mobile requests, got {summary.MobileRequests}");
        Assert.True(summary.DesktopRequests >= 2, $"Expected at least 2 desktop requests, got {summary.DesktopRequests}");
        Assert.True(summary.MobilePercentage > 0, "Mobile percentage should be greater than 0");
        Assert.True(summary.DesktopPercentage > 0, "Desktop percentage should be greater than 0");
    }
}
