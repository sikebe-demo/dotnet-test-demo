using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class CacheHeaderTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CacheHeaderTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_StaticJsFile_ReturnsCacheHeaders()
    {
        // Arrange
        var path = "/js/site.js";

        // Act
        var response = await _client.GetAsync(path);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Cache-Control", response.Headers.Select(h => h.Key));
        
        var cacheControlHeader = response.Headers.GetValues("Cache-Control").FirstOrDefault();
        Assert.NotNull(cacheControlHeader);
        Assert.Contains("public", cacheControlHeader);
        Assert.Contains("max-age", cacheControlHeader);
    }

    [Fact]
    public async Task Get_VersionedStaticFile_ReturnsImmutableCacheHeaders()
    {
        // Arrange
        var path = "/js/site.js?v=test123";

        // Act
        var response = await _client.GetAsync(path);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var cacheControlHeader = response.Headers.GetValues("Cache-Control").FirstOrDefault();
        Assert.NotNull(cacheControlHeader);
        Assert.Contains("public", cacheControlHeader);
        Assert.Contains("max-age=86400", cacheControlHeader);
        Assert.Contains("immutable", cacheControlHeader);
    }

    [Fact]
    public async Task Get_HtmlPage_ReturnsNoCacheHeaders()
    {
        // Arrange
        var path = "/";

        // Act
        var response = await _client.GetAsync(path);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("text/html", response.Content.Headers.ContentType?.ToString() ?? "");
        
        // Check that HTML responses have no-cache headers
        var cacheControlHeader = response.Headers.GetValues("Cache-Control").FirstOrDefault();
        Assert.NotNull(cacheControlHeader);
        Assert.Contains("no-store", cacheControlHeader);
        Assert.Contains("no-cache", cacheControlHeader);
    }

    [Theory]
    [InlineData("/css/site.css")]
    [InlineData("/lib/bootstrap/dist/css/bootstrap.min.css")]
    [InlineData("/lib/jquery/dist/jquery.min.js")]
    public async Task Get_StaticAssets_ReturnsCacheHeaders(string path)
    {
        // Act
        var response = await _client.GetAsync(path);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var cacheControlHeader = response.Headers.GetValues("Cache-Control").FirstOrDefault();
        Assert.NotNull(cacheControlHeader);
        Assert.Contains("public", cacheControlHeader);
        Assert.Contains("max-age", cacheControlHeader);
    }
}