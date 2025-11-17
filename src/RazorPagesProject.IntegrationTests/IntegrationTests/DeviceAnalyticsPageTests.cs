using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class DeviceAnalyticsPageTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeviceAnalyticsPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_DeviceAnalyticsPage_ReturnsSuccessAndCorrectContentType()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/DeviceAnalytics");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8",
            response?.Content?.Headers?.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_DeviceAnalyticsPage_ContainsExpectedContent()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/DeviceAnalytics");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Device Analytics", content);
        Assert.Contains("Access Statistics", content);
        Assert.Contains("Mobile", content);
        Assert.Contains("Tablet", content);
        Assert.Contains("Desktop", content);
        Assert.Contains("Decision Criteria", content);
        Assert.Contains("Privacy Notice", content);
    }
}
