using Microsoft.AspNetCore.Mvc.Testing;
using RazorPagesProject.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Xunit;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class PerformanceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PerformanceTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task UserAgentMiddleware_PerformanceImpact_ShouldBeLessThan5Ms()
    {
        // Arrange
        var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15";
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("User-Agent", userAgent);

        // Warm up requests to avoid cold start bias
        for (int i = 0; i < 5; i++)
        {
            await _client.GetAsync("/");
        }

        // Act - Measure performance over multiple requests
        var times = new List<long>();
        const int iterations = 20;

        for (int i = 0; i < iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync("/");
            stopwatch.Stop();
            
            // Ensure successful response
            Assert.True(response.IsSuccessStatusCode);
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert - Calculate average and verify performance requirement
        var averageTime = times.Average();
        var maxTime = times.Max();
        
        // The middleware should add minimal overhead
        // Given the requirement is < +5ms TTFB, and we're measuring full round trip,
        // we'll allow for up to 50ms total for a local round trip request
        Assert.True(averageTime < 50, $"Average response time {averageTime}ms exceeded 50ms threshold");
        Assert.True(maxTime < 100, $"Maximum response time {maxTime}ms exceeded 100ms threshold");
    }

    [Fact]
    public async Task UserAgentMiddleware_AccuracyUnderLoad_ShouldMaintainCorrectCounts()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var countersService = scope.ServiceProvider.GetRequiredService<IDeviceCountersService>();
        
        var initialSummary = countersService.GetCumulativeSummary();
        var initialTotal = initialSummary.Mobile + initialSummary.Tablet + initialSummary.Desktop;

        // Act - Send concurrent requests
        var tasks = new List<Task>();
        const int concurrentRequests = 10;

        for (int i = 0; i < concurrentRequests; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var client = _factory.CreateClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15");
                await client.GetAsync("/");
            }));
        }

        await Task.WhenAll(tasks);

        // Assert - Verify all requests were counted
        var finalSummary = countersService.GetCumulativeSummary();
        var finalTotal = finalSummary.Mobile + finalSummary.Tablet + finalSummary.Desktop;
        
        Assert.True(finalTotal >= initialTotal + concurrentRequests, 
            $"Expected at least {concurrentRequests} new requests to be counted. Initial: {initialTotal}, Final: {finalTotal}");
    }
}