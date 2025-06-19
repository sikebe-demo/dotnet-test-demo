using OpenQA.Selenium;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

public class GitHubProfilePage(IWebDriver driver, ITestOutputHelper? helper) : PageObjectBase(driver, helper)
{
    public IWebElement UserNameInput => CreateWait().Until(driver =>
        driver.FindElement(By.Id("Input_UserName")));

    public IWebElement SubmitButton => CreateWait().Until(driver =>
        driver.FindElement(By.CssSelector("form#user-profile button[type='submit']")));

    public IWebElement LanguageDropdown => CreateWait().Until(driver =>
        driver.FindElement(By.Id("languageDropdown")));

    public IWebElement Login => CreateWait().Until(driver =>
        driver.FindElement(By.Id("user-login")));

    public IWebElement Name => CreateWait().Until(driver =>
        driver.FindElement(By.Id("name")));

    public IWebElement Company => CreateWait().Until(driver =>
        driver.FindElement(By.Id("company")));

    public GitHubProfilePage ClickShowUserProfileButton()
    {
        var submitButton = CreateWait().Until(driver =>
            driver.FindElement(By.CssSelector("form#user-profile button[type='submit']")));

        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", submitButton);

        // Wait for profile content to load
        CreateWait(TimeSpan.FromSeconds(15)).Until(driver =>
        {
            try
            {
                // Check if any profile elements are present
                return driver.FindElements(By.Id("user-login")).Count != 0 ||
                       driver.FindElements(By.Id("name")).Count != 0 ||
                       driver.FindElements(By.Id("company")).Count != 0;
            }
            catch
            {
                return false;
            }
        });

        return this;
    }

    public void NavigateWithCulture(string culture)
    {
        var url = $"{Constants.BaseUrl}/GitHubProfile?culture={culture}";
        Driver.Navigate().GoToUrl(url);
    }

    public bool HasPageTitle(string expectedTitle)
    {
        return Driver.PageSource.Contains(expectedTitle);
    }

    public bool HasLabels(params string[] expectedLabels)
    {
        var pageSource = Driver.PageSource;
        return expectedLabels.All(label => pageSource.Contains(label));
    }

    public async Task SwitchToJapaneseAsync()
    {
        var wait = CreateWait();

        // Open language dropdown using ID
        var dropdownButton = wait.Until(driver => driver.FindElement(By.Id("languageDropdown")));
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", dropdownButton);

        // Click Japanese option using more specific selector
        var japaneseButton = wait.Until(driver =>
            driver.FindElement(By.XPath("//button[@type='submit' and contains(., '日本語')]")));
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", japaneseButton);

        // Wait for Japanese content to load
        await WaitForConditionAsync(() => Driver.PageSource.Contains("GitHub プロフィール エクスプローラー"));
    }

    public async Task SwitchToEnglishAsync()
    {
        var wait = CreateWait();

        // Open language dropdown using ID
        var dropdownButton = wait.Until(driver => driver.FindElement(By.Id("languageDropdown")));
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", dropdownButton);

        // Click English option using more specific selector
        var englishButton = wait.Until(driver =>
            driver.FindElement(By.XPath("//button[@type='submit' and contains(., 'English')]")));
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", englishButton);

        // Wait for English content to load
        await WaitForConditionAsync(() => Driver.PageSource.Contains("GitHub Profile Explorer"));
    }

    public async Task<GitHubProfilePage> SearchUserAsync(string username)
    {
        var wait = CreateWait();

        var userNameInput = wait.Until(driver => driver.FindElement(By.Id("Input_UserName")));
        var submitButton = wait.Until(driver => driver.FindElement(By.CssSelector("form#user-profile button[type='submit']")));

        userNameInput.Clear();
        userNameInput.SendKeys(username);

        // Use JavaScript click to avoid interactability issues
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", submitButton);

        // Wait for profile content to load by checking for specific ID elements
        await WaitForConditionAsync(() =>
        {
            try
            {
                return Driver.FindElements(By.Id("user-login")).Count != 0 ||
                       Driver.FindElements(By.Id("name")).Count != 0 ||
                       Driver.FindElements(By.Id("company")).Count != 0;
            }
            catch
            {
                return false;
            }
        });

        return this;
    }

    public bool HasProfileContent(string username)
    {
        return Driver.PageSource.Contains(username);
    }

    public bool HasLocalizedContent(string culture)
    {
        var pageSource = Driver.PageSource;
        return culture switch
        {
            "en" => pageSource.Contains("Profile") || pageSource.Contains("GitHub"),
            "ja" => pageSource.Contains("プロフィール") || pageSource.Contains("GitHub"),
            _ => false
        };
    }
}
