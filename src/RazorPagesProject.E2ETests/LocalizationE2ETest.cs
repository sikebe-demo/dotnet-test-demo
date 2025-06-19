using RazorPagesProject.E2ETests.Fixtures;
using RazorPagesProject.E2ETests.PageObjects;
using Xunit.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

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
        // Arrange
        var url = $"{Constants.BaseUrl}/GithubProfile?culture={culture}";

        // Act
        _browser.Driver.Navigate().GoToUrl(url);

        // Assert
        var pageSource = _browser.Driver.PageSource;
        Assert.Contains(expectedTitle, pageSource);

        _helper.WriteLine($"Language: {culture}, Expected: {expectedTitle}");
        _helper.WriteLine($"Page title found in source: {pageSource.Contains(expectedTitle)}");
    }

    [Theory]
    [InlineData("en", "Search", "GitHub Username", "Show Profile")]
    [InlineData("ja", "検索", "GitHubユーザー名", "プロフィールを表示")]
    public void Should_Display_Correct_Labels_Based_On_Language(string culture, string expectedSearchHeader, string expectedUserNameLabel, string expectedSubmitButton)
    {
        // Arrange
        var url = $"{Constants.BaseUrl}/GithubProfile?culture={culture}";

        // Act
        _browser.Driver.Navigate().GoToUrl(url);

        // Assert
        var pageSource = _browser.Driver.PageSource;

        Assert.Contains(expectedSearchHeader, pageSource);
        Assert.Contains(expectedUserNameLabel, pageSource);
        Assert.Contains(expectedSubmitButton, pageSource);

        _helper.WriteLine($"Language: {culture}");
        _helper.WriteLine($"Search header found: {pageSource.Contains(expectedSearchHeader)}");
        _helper.WriteLine($"Username label found: {pageSource.Contains(expectedUserNameLabel)}");
        _helper.WriteLine($"Submit button found: {pageSource.Contains(expectedSubmitButton)}");
    }

    [Fact]
    public void Should_Switch_Language_When_Language_Button_Clicked()
    {
        // Arrange
        _browser.Driver.Navigate().GoToUrl($"{Constants.BaseUrl}/GithubProfile");

        // Act - Switch to Japanese
        var japaneseButton = _browser.Driver.FindElement(By.CssSelector("button[value='ja']"));
        japaneseButton.Click();

        // Assert - Check if Japanese content is displayed
        var pageSource = _browser.Driver.PageSource;
        Assert.Contains("GitHub プロフィール エクスプローラー", pageSource);
        Assert.Contains("検索", pageSource);

        // Act - Switch back to English
        var englishButton = _browser.Driver.FindElement(By.CssSelector("button[value='en']"));
        englishButton.Click();

        // Assert - Check if English content is displayed
        pageSource = _browser.Driver.PageSource;
        Assert.Contains("GitHub Profile Explorer", pageSource);
        Assert.Contains("Search", pageSource);

        _helper.WriteLine("Language switching test completed successfully");
    }

    [Theory]
    [InlineData("en", "Not set")]
    [InlineData("ja", "未設定")]
    public void Should_Display_Profile_With_Correct_Language_After_Search(string culture, string expectedNotSetText)
    {
        // Arrange
        var url = $"{Constants.BaseUrl}/GithubProfile?culture={culture}";
        _browser.Driver.Navigate().GoToUrl(url);

        // Act - Search for a GitHub user
        var userNameInput = _browser.Driver.FindElement(By.Id("Input_UserName"));
        var submitButton = _browser.Driver.FindElement(By.CssSelector("button[type='submit']"));

        userNameInput.Clear();
        userNameInput.SendKeys("octocat");
        submitButton.Click();

        // Wait for the profile to load using explicit wait
        var wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(10));
        wait.Until(driver =>
        {
            var pageSource = driver.PageSource;
            return culture == "en"
                ? pageSource.Contains("Profile") || pageSource.Contains("Login")
                : pageSource.Contains("プロフィール") || pageSource.Contains("ログイン名");
        });

        // Assert
        var pageSource = _browser.Driver.PageSource;

        // Check if the profile labels appear in the correct language
        if (culture == "en")
        {
            Assert.Contains("Profile", pageSource);
            Assert.Contains("Login", pageSource);
            Assert.Contains("Name", pageSource);
            Assert.Contains("Company", pageSource);
        }
        else
        {
            Assert.Contains("プロフィール", pageSource);
            Assert.Contains("ログイン名", pageSource);
            Assert.Contains("名前", pageSource);
            Assert.Contains("会社", pageSource);
        }

        // Note: expectedNotSetText parameter is available for future use when testing empty fields
        _helper.WriteLine($"Profile search test completed for language: {culture}");
        _helper.WriteLine($"Expected 'Not set' text would be: {expectedNotSetText}");
    }
}
