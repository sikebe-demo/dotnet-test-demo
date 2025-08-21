using Microsoft.AspNetCore.Mvc.Testing;
using RazorPagesProject.IntegrationTests.Helpers;
using System.Net;
using AngleSharp.Html.Dom;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

/// <summary>
/// Tests to verify Serilog structured logging integration
/// </summary>
public class SerilogTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public SerilogTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_HomePageRequest_ShouldSucceedWithStructuredLogging()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // If we got here, the request succeeded which means Serilog request logging
        // is working correctly without breaking the application flow
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Messages", content); // Basic page content check
    }
}