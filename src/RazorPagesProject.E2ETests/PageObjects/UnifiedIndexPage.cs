using OpenQA.Selenium;
using RazorPagesProject.E2ETests.Abstractions;
using Xunit.Abstractions;

namespace RazorPagesProject.E2ETests.PageObjects;

/// <summary>
/// Unified Index Page Object that works with both Selenium and Playwright
/// </summary>
public class UnifiedIndexPage : UnifiedPageObjectBase
{
    public UnifiedIndexPage(IBrowserDriver driver, ITestOutputHelper? helper = null) 
        : base(driver, helper) { }

    /// <summary>
    /// Get the new message input element
    /// </summary>
    public async Task<IElementLocator> GetNewMessageInputAsync()
    {
        return await Driver.FindElementAsync(By.Name("Message.Text"));
    }

    /// <summary>
    /// Add a new message to the page
    /// </summary>
    public async Task<UnifiedIndexPage> AddMessageAsync(string message)
    {
        var messageInput = await GetNewMessageInputAsync();
        await messageInput.ClearAsync();
        await messageInput.SendKeysAsync(message);
        
        var submitButton = await Driver.FindElementAsync(By.Id("addMessageBtn"));
        await submitButton.ClickAsync();

        // Wait for the message to appear in the list
        await WaitForConditionAsync(async () =>
        {
            try
            {
                var currentMessages = await Driver.FindElementsAsync(By.ClassName("message-list"));
                foreach (var messageElement in currentMessages)
                {
                    try
                    {
                        var text = await messageElement.GetTextAsync();
                        if (text == message)
                            return true;
                    }
                    catch
                    {
                        // Continue if element is stale
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }, TimeSpan.FromSeconds(15));

        return this;
    }

    /// <summary>
    /// Check if a specific message exists on the page
    /// </summary>
    public async Task<bool> HasMessageAsync(string expectedMessage)
    {
        try
        {
            var currentMessages = await Driver.FindElementsAsync(By.ClassName("message-list"));
            foreach (var messageElement in currentMessages)
            {
                try
                {
                    var text = await messageElement.GetTextAsync();
                    if (text == expectedMessage)
                        return true;
                }
                catch
                {
                    // Continue if element is stale
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Click the add message button
    /// </summary>
    public async Task<UnifiedIndexPage> ClickAddMessageButtonAsync()
    {
        var button = await Driver.FindElementAsync(By.Id("addMessageBtn"));
        await button.ClickAsync();
        return this;
    }
}