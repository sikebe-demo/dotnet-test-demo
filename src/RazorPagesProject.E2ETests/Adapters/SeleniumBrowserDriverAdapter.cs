using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using RazorPagesProject.E2ETests.Abstractions;
using RazorPagesProject.E2ETests.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace RazorPagesProject.E2ETests.Adapters;

/// <summary>
/// Browser driver implementation for Selenium WebDriver
/// </summary>
public class SeleniumBrowserDriverAdapter : IBrowserDriver
{
    private readonly E2ETestConfiguration _configuration;
    private IWebDriver? _driver;
    private bool _disposed = false;

    public SeleniumBrowserDriverAdapter(E2ETestConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task InitializeAsync()
    {
        // Ensure driver can start under proxy environment
        Environment.SetEnvironmentVariable("no_proxy", "localhost");

        var testDllDir = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SeleniumBrowserDriverAdapter))!.Location)!;

        if (_configuration.DownloadDirectory == null)
        {
            var downloadDir = Path.Combine(testDllDir, "download");
            Directory.CreateDirectory(downloadDir);
            _configuration.DownloadDirectory = downloadDir;
        }

        if (_configuration.LogDirectory == null)
        {
            var logDir = Path.Combine(testDllDir, "logs");
            Directory.CreateDirectory(logDir);
            _configuration.LogDirectory = logDir;
        }

        _driver = await Task.FromResult(CreateDriver());
    }

    private IWebDriver CreateDriver()
    {
        return _configuration.Browser switch
        {
            BrowserType.Edge => CreateEdgeDriver(),
            BrowserType.Chrome => CreateChromeDriver(),
            BrowserType.Firefox => CreateFirefoxDriver(),
            _ => throw new NotSupportedException($"Browser {_configuration.Browser} is not supported")
        };
    }

    private IWebDriver CreateEdgeDriver()
    {
        var options = new EdgeOptions();
        
        // Ignore self-signed certificate warnings
        options.AddArgument("--ignore-certificate-errors");
        
        // Disable notifications dialog
        options.AddArgument("--disable-notifications");
        
        options.AddArguments("-inprivate");
        
        // Workaround for https://github.com/SeleniumHQ/selenium/issues/15340
        if (Environment.GetEnvironmentVariable("CI") != "true")
        {
            options.AddArgument("--edge-skip-compat-layer-relaunch");
        }
        
        if (_configuration.Headless && !Debugger.IsAttached)
        {
            options.AddArgument("headless");
        }

        var driver = new EdgeDriver(EdgeDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(60));
        driver.Manage().Timeouts().ImplicitWait = _configuration.Timeout;
        return driver;
    }

    private IWebDriver CreateChromeDriver()
    {
        var options = new ChromeOptions();
        
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--disable-notifications");
        options.AddArgument("--incognito");
        
        if (_configuration.Headless && !Debugger.IsAttached)
        {
            options.AddArgument("--headless");
        }

        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = _configuration.Timeout;
        return driver;
    }

    private IWebDriver CreateFirefoxDriver()
    {
        var options = new FirefoxOptions();
        
        if (_configuration.Headless && !Debugger.IsAttached)
        {
            options.AddArgument("--headless");
        }

        var driver = new FirefoxDriver(options);
        driver.Manage().Timeouts().ImplicitWait = _configuration.Timeout;
        return driver;
    }

    public Task NavigateAsync(string url)
    {
        EnsureDriverInitialized();
        _driver!.Navigate().GoToUrl(url);
        return Task.CompletedTask;
    }

    public Task<string> GetTitleAsync()
    {
        EnsureDriverInitialized();
        return Task.FromResult(_driver!.Title);
    }

    public Task<string> GetUrlAsync()
    {
        EnsureDriverInitialized();
        return Task.FromResult(_driver!.Url);
    }

    public Task<string> GetPageSourceAsync()
    {
        EnsureDriverInitialized();
        return Task.FromResult(_driver!.PageSource);
    }

    public async Task<IElementLocator> FindElementAsync(By selector, TimeSpan? timeout = null)
    {
        EnsureDriverInitialized();
        var wait = new WebDriverWait(_driver!, timeout ?? _configuration.Timeout);
        var element = await Task.FromResult(wait.Until(driver => driver.FindElement(selector)));
        return new SeleniumElementLocator(element);
    }

    public async Task<IList<IElementLocator>> FindElementsAsync(By selector, TimeSpan? timeout = null)
    {
        EnsureDriverInitialized();
        var elements = await Task.FromResult(_driver!.FindElements(selector));
        return elements.Select(e => (IElementLocator)new SeleniumElementLocator(e)).ToList();
    }

    public async Task<bool> ElementExistsAsync(By selector, TimeSpan? timeout = null)
    {
        EnsureDriverInitialized();
        try
        {
            var wait = new WebDriverWait(_driver!, timeout ?? TimeSpan.FromMilliseconds(500));
            await Task.FromResult(wait.Until(driver => driver.FindElement(selector)));
            return true;
        }
        catch (WebDriverTimeoutException)
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
        EnsureDriverInitialized();
        var screenshot = ((ITakesScreenshot)_driver!).GetScreenshot();
        return await Task.FromResult(screenshot.AsByteArray);
    }

    public async Task<object> ExecuteScriptAsync(string script, params object[] args)
    {
        EnsureDriverInitialized();
        var jsExecutor = (IJavaScriptExecutor)_driver!;
        return await Task.FromResult(jsExecutor.ExecuteScript(script, args) ?? new object());
    }

    public object GetNativeDriver()
    {
        EnsureDriverInitialized();
        return _driver!;
    }

    private void EnsureDriverInitialized()
    {
        if (_driver == null)
            throw new InvalidOperationException("Driver has not been initialized. Call InitializeAsync first.");
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
                // Managed resources
            }

            // Take final screenshot for troubleshooting
            try
            {
                if (_driver != null && _configuration.LogDirectory != null)
                {
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                    var file = Path.Combine(_configuration.LogDirectory, $"{DateTime.Now:yyyyMMddss}.png");
                    screenshot.SaveAsFile(file);
                }
            }
            catch
            {
                // Ignore errors during cleanup
            }

            _driver?.Quit();
            _driver?.Dispose();
            _disposed = true;
        }
    }

    ~SeleniumBrowserDriverAdapter()
    {
        Dispose(false);
    }
}