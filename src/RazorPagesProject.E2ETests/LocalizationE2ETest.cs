using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests;

public class LocalizationE2ETest : IClassFixture<EdgeFixture>
{
    private readonly BrowserFixture _browser;
    private readonly ITestOutputHelper _helper;
    private readonly GitHubProfilePage _githubProfilePage;

    public LocalizationE2ETest(EdgeFixture edgeFixture, ITestOutputHelper helper)
    {
        _browser = edgeFixture;
        _helper = helper;
        _githubProfilePage = new GitHubProfilePage(_browser.Driver, _helper);
    }

    [Theory]
    [InlineData("en", "GitHub Profile Explorer")]
    [InlineData("ja", "GitHub プロフィール エクスプローラー")]
    public void Should_Display_Correct_Page_Title_Based_On_Language(string culture, string expectedTitle)
    {
        // Arrange, Act
        _githubProfilePage.NavigateWithCulture(culture);

        // Assert
        Assert.True(_githubProfilePage.HasPageTitle(expectedTitle),
            $"Expected page title '{expectedTitle}' not found for culture '{culture}'");

        _helper.WriteLine($"Language: {culture}, Expected: {expectedTitle}");
    }

    [Theory]
    [InlineData("en", "Search", "GitHub Username", "Show Profile")]
    [InlineData("ja", "検索", "GitHubユーザー名", "プロフィールを表示")]
    public void Should_Display_Correct_Labels_Based_On_Language(string culture, string expectedSearchHeader, string expectedUserNameLabel, string expectedSubmitButton)
    {
        // Arrange, Act
        _githubProfilePage.NavigateWithCulture(culture);

        // Assert
        Assert.True(_githubProfilePage.HasLabels(expectedSearchHeader, expectedUserNameLabel, expectedSubmitButton),
            $"Not all expected labels found for culture '{culture}'");

        _helper.WriteLine($"Language: {culture} - All labels found successfully");
    }

    [Fact]
    public async Task Should_Switch_Language_When_Language_Button_Clicked()
    {
        // Arrange
        _githubProfilePage.Driver.Navigate().GoToUrl($"{Constants.BaseUrl}/GithubProfile");

        // Act & Assert - Switch to Japanese
        await _githubProfilePage.SwitchToJapaneseAsync();
        Assert.True(_githubProfilePage.HasLabels("GitHub プロフィール エクスプローラー", "検索"),
            "Japanese content not found after switching to Japanese");

        // Act & Assert - Switch back to English
        await _githubProfilePage.SwitchToEnglishAsync();
        Assert.True(_githubProfilePage.HasLabels("GitHub Profile Explorer", "Search"),
            "English content not found after switching to English");

        _helper.WriteLine("Language switching test completed successfully");
    }

    [Theory]
    [InlineData("en", "Not set")]
    [InlineData("ja", "未設定")]
    public async Task Should_Display_Profile_With_Correct_Language_After_Search(string culture, string expectedNotSetText)
    {
        // Arrange
        _githubProfilePage.NavigateWithCulture(culture);

        // Act
        await _githubProfilePage.SearchUserAsync("octocat");

        // Assert
        Assert.True(_githubProfilePage.HasProfileContent("octocat"),
            "Username 'octocat' not found in search results");

        Assert.True(_githubProfilePage.HasLocalizedContent(culture),
            $"Localized content not found for culture '{culture}'");

        _helper.WriteLine($"Profile search test completed for language: {culture}");
        _helper.WriteLine($"Expected 'Not set' text would be: {expectedNotSetText}");
    }
}
