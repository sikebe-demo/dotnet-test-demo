using RazorPagesProject.E2ETests.Fixtures;
using Selenium.Axe;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests;

public class AccessibilityTest : IClassFixture<EdgeFixture>
{
    private readonly BrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;

    public AccessibilityTest(EdgeFixture browserFixture, ITestOutputHelper helper)
    {
        _browserFixture = browserFixture;
        _helper = helper;
    }

    [Fact]
    public void Homepage_Should_Have_No_Accessibility_Violations()
    {
        // Arrange
        _browserFixture.Driver.Navigate().GoToUrl($"{Constants.BaseUrl}?culture=en");

        // Act
        AxeResult results = _browserFixture.Driver.Analyze();

        // Assert - Output detailed violation information for debugging
        if (results.Violations?.Any() == true)
        {
            _helper.WriteLine($"Found {results.Violations.Length} accessibility violations:");
            foreach (var violation in results.Violations)
            {
                _helper.WriteLine($"- {violation.Id}: {violation.Description}");
                _helper.WriteLine($"  Impact: {violation.Impact}");
                _helper.WriteLine($"  Help: {violation.Help}");
                if (violation.Nodes?.Any() == true)
                {
                    foreach (var node in violation.Nodes)
                    {
                        _helper.WriteLine($"  Node: {node.Html}");
                        var targets = node.Target?.Select(t => t.ToString()) ?? new List<string>();
                        _helper.WriteLine($"  Target: {string.Join(", ", targets)}");
                    }
                }
                _helper.WriteLine("");
            }
        }

        Assert.Empty(results.Violations);
    }

    [Fact]
    public void ContactPage_Should_Have_No_Accessibility_Violations()
    {
        // Arrange
        _browserFixture.Driver.Navigate().GoToUrl($"{Constants.BaseUrl}/Contact?culture=en");

        // Act
        AxeResult results = _browserFixture.Driver.Analyze();

        // Assert
        Assert.Empty(results.Violations);
    }

    [Fact]
    public void GitHubProfilePage_Should_Have_No_Accessibility_Violations()
    {
        // Arrange
        _browserFixture.Driver.Navigate().GoToUrl($"{Constants.BaseUrl}/GitHubProfile?culture=en");

        // Act
        AxeResult results = _browserFixture.Driver.Analyze();

        // Assert
        Assert.Empty(results.Violations);
    }
}