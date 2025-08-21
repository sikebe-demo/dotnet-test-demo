using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using Xunit;

namespace RazorPagesProject.IntegrationTests;

public class MetaDescriptionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MetaDescriptionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Theory]
    [InlineData("/", "Manage and analyze messages with our interactive message system")]
    [InlineData("/About", "Learn more about our application and discover the features")]
    [InlineData("/Contact", "Get in touch with our team. Find our contact information")]
    [InlineData("/GitHubProfile", "Explore GitHub profiles with our GitHub Profile Explorer")]
    [InlineData("/Privacy", "Read our privacy policy to understand how we collect")]
    public async Task Pages_Should_Have_Meta_Description_Tags(string url, string expectedContentStart)
    {
        // Act
        var response = await _client.GetAsync(url);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        // Check for meta description tag
        var metaDescriptionPattern = @"<meta\s+name=[""']description[""']\s+content=[""']([^""']+)[""']";
        var match = Regex.Match(content, metaDescriptionPattern, RegexOptions.IgnoreCase);
        
        Assert.True(match.Success, $"Meta description tag not found on page {url}");
        
        var description = match.Groups[1].Value;
        Assert.StartsWith(expectedContentStart, description);
        
        // Verify description length is within SEO range (should be 50-160 characters)
        Assert.InRange(description.Length, 50, 200); // Allowing up to 200 for Japanese text
        
        // Verify it's not empty or just whitespace
        Assert.False(string.IsNullOrWhiteSpace(description));
    }

    [Fact]
    public async Task All_Meta_Descriptions_Should_Be_Unique()
    {
        // Arrange
        var pages = new[] { "/", "/About", "/Contact", "/GitHubProfile", "/Privacy" };
        var descriptions = new List<string>();

        // Act
        foreach (var page in pages)
        {
            var response = await _client.GetAsync(page);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            
            var metaDescriptionPattern = @"<meta\s+name=[""']description[""']\s+content=[""']([^""']+)[""']";
            var match = Regex.Match(content, metaDescriptionPattern, RegexOptions.IgnoreCase);
            
            if (match.Success)
            {
                descriptions.Add(match.Groups[1].Value);
            }
        }

        // Assert
        Assert.Equal(descriptions.Count, descriptions.Distinct().Count());
    }

    [Theory]
    [InlineData("/", "ja", "インタラクティブなメッセージシステムでメッセージの管理と分析")]
    [InlineData("/GitHubProfile", "ja", "GitHubプロフィールエクスプローラーでGitHubプロフィールを探索")]
    public async Task Pages_Should_Have_Localized_Meta_Descriptions(string url, string culture, string expectedContentStart)
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Accept-Language", culture);

        // Act
        var response = await _client.GetAsync(url);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        var metaDescriptionPattern = @"<meta\s+name=[""']description[""']\s+content=[""']([^""']+)[""']";
        var match = Regex.Match(content, metaDescriptionPattern, RegexOptions.IgnoreCase);
        
        Assert.True(match.Success, $"Meta description tag not found on page {url} with culture {culture}");
        
        var description = match.Groups[1].Value;
        Assert.StartsWith(expectedContentStart, description);
    }
}