using Microsoft.Playwright;
using OpenQA.Selenium;
using RazorPagesProject.E2ETests.Abstractions;
using RazorPagesProject.E2ETests.Configuration;
using ConfigBrowserType = RazorPagesProject.E2ETests.Configuration.BrowserType;

namespace RazorPagesProject.E2ETests.Adapters;

/// <summary>
/// Browser driver implementation for Playwright
/// </summary>
public class PlaywrightBrowserDriverAdapter : IBrowserDriver
{
    private readonly E2ETestConfiguration _configuration;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private bool _disposed = false;

    public PlaywrightBrowserDriverAdapter(E2ETestConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        
        var browserType = _configuration.Browser switch
        {
            ConfigBrowserType.Edge => _playwright.Chromium, // Edge uses Chromium engine
            ConfigBrowserType.Chrome => _playwright.Chromium,
            ConfigBrowserType.Firefox => _playwright.Firefox,
            _ => throw new NotSupportedException($"Browser {_configuration.Browser} is not supported")
        };

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _configuration.Headless && !System.Diagnostics.Debugger.IsAttached,
            Args = GetBrowserArgs()
        };

        _browser = await browserType.LaunchAsync(launchOptions);

        var contextOptions = new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        };

        var context = await _browser.NewContextAsync(contextOptions);
        context.SetDefaultTimeout((float)_configuration.Timeout.TotalMilliseconds);
        
        _page = await context.NewPageAsync();
    }

    private string[] GetBrowserArgs()
    {
        var args = new List<string>
        {
            "--ignore-certificate-errors",
            "--disable-notifications"
        };

        if (_configuration.Browser == ConfigBrowserType.Edge)
        {
            args.Add("--edge-skip-compat-layer-relaunch");
        }

        return args.ToArray();
    }

    public async Task NavigateAsync(string url)
    {
        EnsurePageInitialized();
        await _page!.GotoAsync(url);
    }

    public async Task<string> GetTitleAsync()
    {
        EnsurePageInitialized();
        return await _page!.TitleAsync();
    }

    public async Task<string> GetUrlAsync()
    {
        EnsurePageInitialized();
        return _page!.Url;
    }

    public async Task<string> GetPageSourceAsync()
    {
        EnsurePageInitialized();
        return await _page!.ContentAsync();
    }

    public async Task<IElementLocator> FindElementAsync(By selector, TimeSpan? timeout = null)
    {
        EnsurePageInitialized();
        var playwrightSelector = ConvertSeleniumSelector(selector);
        var locator = _page!.Locator(playwrightSelector);
        
        // Wait for element to exist
        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions 
            { 
                Timeout = (float)(timeout ?? _configuration.Timeout).TotalMilliseconds 
            });
        }
        catch (TimeoutException)
        {
            throw new NoSuchElementException($"Element not found: {selector}");
        }
        
        return new PlaywrightElementLocator(locator);
    }

    public async Task<IList<IElementLocator>> FindElementsAsync(By selector, TimeSpan? timeout = null)
    {
        EnsurePageInitialized();
        var playwrightSelector = ConvertSeleniumSelector(selector);
        var locators = await _page!.Locator(playwrightSelector).AllAsync();
        
        return locators.Select(l => (IElementLocator)new PlaywrightElementLocator(_page.Locator(playwrightSelector).Nth(locators.ToList().IndexOf(l)))).ToList();
    }

    public async Task<bool> ElementExistsAsync(By selector, TimeSpan? timeout = null)
    {
        EnsurePageInitialized();
        var playwrightSelector = ConvertSeleniumSelector(selector);
        var locator = _page!.Locator(playwrightSelector);
        
        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions 
            { 
                Timeout = (float)(timeout ?? TimeSpan.FromMilliseconds(500)).TotalMilliseconds 
            });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task WaitForConditionAsync(Func<IBrowserDriver, Task<bool>> condition, TimeSpan? timeout = null)
    {
        var endTime = DateTime.UtcNow.Add(timeout ?? _configuration.Timeout);
        
        while (DateTime.UtcNow < endTime)
        {
            if (await condition(this))
                return;
            
            await Task.Delay(100);
        }
        
        throw new TimeoutException("Condition was not met within the specified timeout.");
    }

    public async Task<byte[]> TakeScreenshotAsync()
    {
        EnsurePageInitialized();
        return await _page!.ScreenshotAsync();
    }

    public async Task<object> ExecuteScriptAsync(string script, params object[] args)
    {
        EnsurePageInitialized();
        return await _page!.EvaluateAsync(script, args.Length > 0 ? args[0] : null) ?? new object();
    }

    public object GetNativeDriver()
    {
        EnsurePageInitialized();
        return _page!;
    }

    /// <summary>
    /// Convert Selenium By selector to Playwright selector string
    /// </summary>
    private string ConvertSeleniumSelector(By selector)
    {
        // Extract the selector value using reflection since By doesn't expose it directly
        var selectorString = selector.ToString();
        
        if (selectorString.StartsWith("By.Id: "))
        {
            var id = selectorString.Substring("By.Id: ".Length);
            return $"#{id}";
        }
        else if (selectorString.StartsWith("By.ClassName: "))
        {
            var className = selectorString.Substring("By.ClassName: ".Length);
            return $".{className}";
        }
        else if (selectorString.StartsWith("By.CssSelector: "))
        {
            return selectorString.Substring("By.CssSelector: ".Length);
        }
        else if (selectorString.StartsWith("By.XPath: "))
        {
            return selectorString.Substring("By.XPath: ".Length);
        }
        else if (selectorString.StartsWith("By.LinkText: "))
        {
            var linkText = selectorString.Substring("By.LinkText: ".Length);
            return $"a:has-text(\"{linkText}\")";
        }
        else if (selectorString.StartsWith("By.PartialLinkText: "))
        {
            var partialLinkText = selectorString.Substring("By.PartialLinkText: ".Length);
            return $"a:has-text(\"{partialLinkText}\")";
        }
        else if (selectorString.StartsWith("By.Name: "))
        {
            var name = selectorString.Substring("By.Name: ".Length);
            return $"[name=\"{name}\"]";
        }
        else if (selectorString.StartsWith("By.TagName: "))
        {
            return selectorString.Substring("By.TagName: ".Length);
        }
        
        // Fallback - return as CSS selector
        return selectorString;
    }

    private void EnsurePageInitialized()
    {
        if (_page == null)
            throw new InvalidOperationException("Page has not been initialized. Call InitializeAsync first.");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Take final screenshot for troubleshooting
                try
                {
                    if (_page != null && _configuration.LogDirectory != null)
                    {
                        var screenshot = _page.ScreenshotAsync().GetAwaiter().GetResult();
                        var file = Path.Combine(_configuration.LogDirectory, $"{DateTime.Now:yyyyMMddss}.png");
                        File.WriteAllBytes(file, screenshot);
                    }
                }
                catch
                {
                    // Ignore errors during cleanup
                }

                _page?.CloseAsync().GetAwaiter().GetResult();
                _browser?.CloseAsync().GetAwaiter().GetResult();
                _playwright?.Dispose();
            }

            _disposed = true;
        }
    }

    ~PlaywrightBrowserDriverAdapter()
    {
        Dispose(false);
    }
}