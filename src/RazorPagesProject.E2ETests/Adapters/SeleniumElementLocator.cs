using OpenQA.Selenium;
using RazorPagesProject.E2ETests.Abstractions;

namespace RazorPagesProject.E2ETests.Adapters;

/// <summary>
/// Element locator implementation for Selenium WebDriver
/// </summary>
public class SeleniumElementLocator : IElementLocator
{
    private readonly IWebElement _element;

    public SeleniumElementLocator(IWebElement element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public Task ClickAsync()
    {
        _element.Click();
        return Task.CompletedTask;
    }

    public Task SendKeysAsync(string text)
    {
        _element.SendKeys(text);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _element.Clear();
        return Task.CompletedTask;
    }

    public Task<string> GetTextAsync()
    {
        return Task.FromResult(_element.Text);
    }

    public Task<string> GetAttributeAsync(string attributeName)
    {
        return Task.FromResult(_element.GetAttribute(attributeName) ?? string.Empty);
    }

    public Task<bool> IsDisplayedAsync()
    {
        return Task.FromResult(_element.Displayed);
    }

    public Task<bool> IsEnabledAsync()
    {
        return Task.FromResult(_element.Enabled);
    }

    public Task<bool> IsSelectedAsync()
    {
        return Task.FromResult(_element.Selected);
    }

    public object GetNativeElement()
    {
        return _element;
    }
}