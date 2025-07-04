using RazorPagesProject.E2ETests.Abstractions;
using RazorPagesProject.E2ETests.Configuration;

namespace RazorPagesProject.E2ETests.Fixtures;

/// <summary>
/// Unified browser fixture that supports both Selenium and Playwright
/// </summary>
public class UnifiedBrowserFixture : IDisposable
{
    private readonly E2ETestConfiguration _configuration;
    private readonly BrowserDriverFactory _factory;
    private IBrowserDriver? _driver;
    private bool _disposed = false;

    public UnifiedBrowserFixture()
    {
        _configuration = ConfigurationHelper.LoadConfiguration();
        _factory = new BrowserDriverFactory(_configuration);
    }

    public UnifiedBrowserFixture(E2ETestConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _factory = new BrowserDriverFactory(_configuration);
    }

    /// <summary>
    /// Get the unified browser driver instance
    /// </summary>
    public async Task<IBrowserDriver> GetDriverAsync()
    {
        if (_driver == null)
        {
            _driver = await _factory.CreateDriverAsync();
        }
        return _driver;
    }

    /// <summary>
    /// Get the current framework being used
    /// </summary>
    public TestFramework Framework => _configuration.Framework;

    /// <summary>
    /// Get the current browser type
    /// </summary>
    public BrowserType Browser => _configuration.Browser;

    /// <summary>
    /// Get the configuration
    /// </summary>
    public E2ETestConfiguration Configuration => _configuration;

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
                _driver?.Dispose();
            }
            _disposed = true;
        }
    }

    ~UnifiedBrowserFixture()
    {
        Dispose(false);
    }
}