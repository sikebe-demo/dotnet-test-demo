using Microsoft.AspNetCore.Mvc.Testing;
using RazorPagesProject.IntegrationTests.Helpers;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class AccessibilityTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AccessibilityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GitHubProfile_DecorativeIcons_HaveAriaHidden()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/GitHubProfile");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert - Check that decorative icons have aria-hidden="true"
        
        // Page header GitHub icon (decorative)
        var pageHeaderIcon = document.QuerySelector("h1.page-title i.fab.fa-github");
        Assert.NotNull(pageHeaderIcon);
        Assert.Equal("true", pageHeaderIcon.GetAttribute("aria-hidden"));

        // Search section header icon (decorative)
        var searchHeaderIcon = document.QuerySelector(".github-header h3 i.fas.fa-search");
        Assert.NotNull(searchHeaderIcon);
        Assert.Equal("true", searchHeaderIcon.GetAttribute("aria-hidden"));

        // Profile section header icon (decorative)
        var profileHeaderIcon = document.QuerySelector(".github-header h3 i.fas.fa-user");
        // This icon only appears when a profile is displayed, so it might not be present in the empty state
    }

    [Fact]
    public async Task GitHubProfile_FunctionalIcons_HaveAccessibleNames()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/GitHubProfile");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert - Check that functional icons have aria-label or other accessible names
        
        // GitHub input field icon (functional)
        var inputIcon = document.QuerySelector(".input-group-text i.fab.fa-github");
        Assert.NotNull(inputIcon);
        Assert.NotNull(inputIcon.GetAttribute("aria-label"));
        Assert.False(string.IsNullOrEmpty(inputIcon.GetAttribute("aria-label")));

        // Search button icon (functional)
        var searchButtonIcon = document.QuerySelector("button.search-btn i.fas.fa-search");
        Assert.NotNull(searchButtonIcon);
        Assert.NotNull(searchButtonIcon.GetAttribute("aria-label"));
        Assert.False(string.IsNullOrEmpty(searchButtonIcon.GetAttribute("aria-label")));
    }

    [Fact]
    public async Task LanguageSwitcher_Icons_HaveAccessibleNames()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert - Check that language switcher icons have aria-label
        
        // Globe icon in language switcher button (functional)
        var globeIcon = document.QuerySelector(".language-switcher button i.fas.fa-globe");
        Assert.NotNull(globeIcon);
        Assert.NotNull(globeIcon.GetAttribute("aria-label"));
        Assert.False(string.IsNullOrEmpty(globeIcon.GetAttribute("aria-label")));

        // Flag icons in dropdown options (functional)
        var flagIcons = document.QuerySelectorAll(".language-switcher .dropdown-item i[class*='fa-flag']");
        Assert.True(flagIcons.Length > 0);
        
        foreach (var flagIcon in flagIcons)
        {
            Assert.NotNull(flagIcon.GetAttribute("aria-label"));
            Assert.False(string.IsNullOrEmpty(flagIcon.GetAttribute("aria-label")));
        }
    }
}