using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesProject.Services;
using RazorPagesProject.Models;
using Xunit;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class UserAgentTrackingMiddlewareTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserAgentTrackingMiddlewareTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15", DeviceType.Mobile)]
    [InlineData("Mozilla/5.0 (iPad; CPU OS 15_0 like Mac OS X) AppleWebKit/605.1.15", DeviceType.Tablet)]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36", DeviceType.Desktop)]
    public async Task UserAgentTrackingMiddleware_DifferentUserAgents_CountsCorrectly(string userAgent, DeviceType expectedDeviceType)
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var countersService = scope.ServiceProvider.GetRequiredService<IDeviceCountersService>();
        
        // Get initial counts
        var initialSummary = countersService.GetCumulativeSummary();
        var initialCount = GetDeviceCount(initialSummary, expectedDeviceType);

        // Act
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        
        var response = await _client.GetAsync("/");

        // Assert
        var finalSummary = countersService.GetCumulativeSummary();
        var finalCount = GetDeviceCount(finalSummary, expectedDeviceType);
        
        Assert.True(finalCount > initialCount, $"Expected {expectedDeviceType} count to increase from {initialCount} to {finalCount}");
    }

    [Fact]
    public async Task UserAgentTrackingMiddleware_MultipleRequests_AccumulatesCorrectly()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var countersService = scope.ServiceProvider.GetRequiredService<IDeviceCountersService>();
        
        var initialSummary = countersService.GetCumulativeSummary();
        var initialMobileCount = initialSummary.Mobile;
        var initialTabletCount = initialSummary.Tablet;
        var initialDesktopCount = initialSummary.Desktop;

        // Act - Send requests with different User-Agents
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15");
        await _client.GetAsync("/");

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPad; CPU OS 15_0 like Mac OS X) AppleWebKit/605.1.15");
        await _client.GetAsync("/");

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        await _client.GetAsync("/");

        // Assert
        var finalSummary = countersService.GetCumulativeSummary();
        
        Assert.True(finalSummary.Mobile > initialMobileCount, "Mobile count should have increased");
        Assert.True(finalSummary.Tablet > initialTabletCount, "Tablet count should have increased");
        Assert.True(finalSummary.Desktop > initialDesktopCount, "Desktop count should have increased");
    }

    private static int GetDeviceCount(DeviceUsageSummary summary, DeviceType deviceType)
    {
        return deviceType switch
        {
            DeviceType.Mobile => summary.Mobile,
            DeviceType.Tablet => summary.Tablet,
            DeviceType.Desktop => summary.Desktop,
            _ => 0
        };
    }
}