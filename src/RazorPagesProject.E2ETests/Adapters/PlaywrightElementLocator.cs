using Microsoft.Playwright;
using RazorPagesProject.E2ETests.Abstractions;

namespace RazorPagesProject.E2ETests.Adapters;

/// <summary>
/// Element locator implementation for Playwright
/// </summary>
public class PlaywrightElementLocator : IElementLocator
{
    private readonly ILocator _locator;

    public PlaywrightElementLocator(ILocator locator)
    {
        _locator = locator ?? throw new ArgumentNullException(nameof(locator));
    }

    public async Task ClickAsync()
    {
        await _locator.ClickAsync();
    }

    public async Task SendKeysAsync(string text)
    {
        await _locator.FillAsync(text);
    }

    public async Task ClearAsync()
    {
        await _locator.ClearAsync();
    }

    public async Task<string> GetTextAsync()
    {
        return await _locator.InnerTextAsync();
    }

    public async Task<string> GetAttributeAsync(string attributeName)
    {
        return await _locator.GetAttributeAsync(attributeName) ?? string.Empty;
    }

    public async Task<bool> IsDisplayedAsync()
    {
        return await _locator.IsVisibleAsync();
    }

    public async Task<bool> IsEnabledAsync()
    {
        return await _locator.IsEnabledAsync();
    }

    public async Task<bool> IsSelectedAsync()
    {
        try
        {
            return await _locator.IsCheckedAsync();
        }
        catch
        {
            return false;
        }
    }

    public object GetNativeElement()
    {
        return _locator;
    }
}