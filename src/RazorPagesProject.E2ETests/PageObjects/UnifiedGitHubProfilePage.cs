using OpenQA.Selenium;
using RazorPagesProject.E2ETests.Abstractions;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

/// <summary>
/// Unified GitHub Profile Page Object that works with both Selenium and Playwright
/// </summary>
public class UnifiedGitHubProfilePage : UnifiedPageObjectBase
{
    public UnifiedGitHubProfilePage(IBrowserDriver driver, ITestOutputHelper? helper = null) 
        : base(driver, helper) { }

    /// <summary>
    /// Get the username input element
    /// </summary>
    public async Task<IElementLocator> GetUserNameInputAsync()
    {
        return await Driver.FindElementAsync(By.Id("Input_UserName"));
    }

    /// <summary>
    /// Get the submit button
    /// </summary>
    public async Task<IElementLocator> GetSubmitButtonAsync()
    {
        return await Driver.FindElementAsync(By.CssSelector("form#user-profile button[type='submit']"));
    }

    /// <summary>
    /// Get the login display element
    /// </summary>
    public async Task<IElementLocator> GetLoginAsync()
    {
        return await Driver.FindElementAsync(By.Id("user-login"));
    }

    /// <summary>
    /// Get the name display element
    /// </summary>
    public async Task<IElementLocator> GetNameAsync()
    {
        return await Driver.FindElementAsync(By.Id("name"));
    }

    /// <summary>
    /// Get the company display element
    /// </summary>
    public async Task<IElementLocator> GetCompanyAsync()
    {
        return await Driver.FindElementAsync(By.Id("company"));
    }

    /// <summary>
    /// Search for a user profile
    /// </summary>
    public async Task<UnifiedGitHubProfilePage> SearchUserAsync(string username)
    {
        var userNameInput = await GetUserNameInputAsync();
        var submitButton = await GetSubmitButtonAsync();

        await userNameInput.ClearAsync();
        await userNameInput.SendKeysAsync(username);

        // Use script execution for reliable clicking
        var nativeElement = submitButton.GetNativeElement();
        await Driver.ExecuteScriptAsync("arguments[0].click();", nativeElement);

        // Wait for profile content to load by checking for specific ID elements
        await WaitForConditionAsync(async () =>
        {
            try
            {
                return await Driver.ElementExistsAsync(By.Id("user-login")) ||
                       await Driver.ElementExistsAsync(By.Id("name")) ||
                       await Driver.ElementExistsAsync(By.Id("company"));
            }
            catch
            {
                return false;
            }
        }, TimeSpan.FromSeconds(15));

        return this;
    }

    /// <summary>
    /// Click the show user profile button
    /// </summary>
    public async Task ClickShowUserProfileButtonAsync()
    {
        var submitButton = await GetSubmitButtonAsync();
        await submitButton.ClickAsync();
    }

    /// <summary>
    /// Navigate to the page with specific culture
    /// </summary>
    public async Task NavigateWithCultureAsync(string culture)
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "https://localhost:7072";
        var url = $"{baseUrl}/GitHubProfile?culture={culture}";
        await Driver.NavigateAsync(url);
    }

    /// <summary>
    /// Check if the page has the expected title
    /// </summary>
    public async Task<bool> HasPageTitleAsync(string expectedTitle)
    {
        var title = await Driver.GetTitleAsync();
        return title.Contains(expectedTitle);
    }

    /// <summary>
    /// Check if the page has specific labels (for localization testing)
    /// </summary>
    public async Task<bool> HasLabelsAsync(params string[] expectedLabels)
    {
        var pageSource = await Driver.GetPageSourceAsync();
        return expectedLabels.All(label => pageSource.Contains(label));
    }

    /// <summary>
    /// Switch to Japanese language
    /// </summary>
    public async Task SwitchToJapaneseAsync()
    {
        // Open language dropdown using ID
        var dropdownButton = await Driver.FindElementAsync(By.Id("languageDropdown"));
        var nativeDropdown = dropdownButton.GetNativeElement();
        await Driver.ExecuteScriptAsync("arguments[0].click();", nativeDropdown);

        // Click Japanese option using more specific selector
        var japaneseButton = await Driver.FindElementAsync(By.XPath("//button[@type='submit' and contains(., '日本語')]"));
        var nativeJapanese = japaneseButton.GetNativeElement();
        await Driver.ExecuteScriptAsync("arguments[0].click();", nativeJapanese);

        // Wait for Japanese content to load
        await WaitForConditionAsync(async () => 
        {
            var pageSource = await Driver.GetPageSourceAsync();
            return pageSource.Contains("GitHub プロフィール エクスプローラー");
        });
    }

    /// <summary>
    /// Switch to English language
    /// </summary>
    public async Task SwitchToEnglishAsync()
    {
        // Open language dropdown using ID
        var dropdownButton = await Driver.FindElementAsync(By.Id("languageDropdown"));
        var nativeDropdown = dropdownButton.GetNativeElement();
        await Driver.ExecuteScriptAsync("arguments[0].click();", nativeDropdown);

        // Click English option using more specific selector
        var englishButton = await Driver.FindElementAsync(By.XPath("//button[@type='submit' and contains(., 'English')]"));
        var nativeEnglish = englishButton.GetNativeElement();
        await Driver.ExecuteScriptAsync("arguments[0].click();", nativeEnglish);

        // Wait for English content to load
        await WaitForConditionAsync(async () => 
        {
            var pageSource = await Driver.GetPageSourceAsync();
            return pageSource.Contains("GitHub Profile Explorer");
        });
    }
}