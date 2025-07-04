# Unified E2E Testing Framework

This implementation provides a unified E2E testing framework that supports both Selenium and Playwright with maximum code sharing.

## 🏗️ Architecture

### Core Components

- **Abstractions**: Common interfaces for browser operations (`IBrowserDriver`, `IElementLocator`)
- **Adapters**: Framework-specific implementations (Selenium and Playwright)
- **Configuration**: Settings management for framework selection
- **Unified Page Objects**: Framework-agnostic page object implementations
- **Fixtures**: Test fixtures that work with both frameworks

### Project Structure

```
RazorPagesProject.E2ETests/
├── Abstractions/                    # Common interfaces
│   ├── IBrowserDriver.cs           # Unified browser driver interface
│   ├── IElementLocator.cs          # Unified element operations interface
│   └── IPageObjectBase.cs          # Base page object interface
├── Adapters/                       # Framework implementations
│   ├── SeleniumBrowserDriverAdapter.cs    # Selenium implementation
│   ├── SeleniumElementLocator.cs          # Selenium element wrapper
│   ├── PlaywrightBrowserDriverAdapter.cs  # Playwright implementation
│   └── PlaywrightElementLocator.cs        # Playwright element wrapper
├── Configuration/                  # Configuration management
│   ├── E2ETestConfiguration.cs     # Configuration model
│   ├── ConfigurationHelper.cs      # Configuration loader
│   └── BrowserDriverFactory.cs     # Driver factory
├── Fixtures/                       # Test fixtures
│   └── UnifiedBrowserFixture.cs    # Unified browser fixture
├── PageObjects/                    # Page object implementations
│   ├── UnifiedPageObjectBase.cs    # Base page object class
│   ├── UnifiedIndexPage.cs         # Index page object
│   └── UnifiedGitHubProfilePage.cs # GitHub profile page object
└── appsettings.json                # Default configuration
```

## ⚙️ Configuration

### Framework Selection

The framework can be selected through multiple methods:

1. **appsettings.json** (default configuration)
2. **Environment variables** (highest priority)
3. **Code configuration** (for specific tests)

### Configuration Files

**appsettings.json** (Selenium default):
```json
{
  "E2ETests": {
    "Framework": "Selenium",
    "Browser": "Edge", 
    "Headless": true,
    "Timeout": "00:00:10",
    "BaseUrl": "https://localhost:7072"
  }
}
```

**appsettings.playwright.json** (Playwright configuration):
```json
{
  "E2ETests": {
    "Framework": "Playwright",
    "Browser": "Edge",
    "Headless": true, 
    "Timeout": "00:00:10",
    "BaseUrl": "https://localhost:7072"
  }
}
```

### Environment Variables

Override configuration using environment variables with `E2E_` prefix:

```bash
# Select framework
export E2E_FRAMEWORK=Playwright
export E2E_BROWSER=Chrome
export E2E_HEADLESS=false
export E2E_BASE_URL=http://localhost:5000
```

## 🧪 Usage Examples

### 1. Using Default Configuration (Selenium)

```csharp
public class MyE2ETest : IClassFixture<UnifiedBrowserFixture>
{
    private readonly UnifiedBrowserFixture _browserFixture;
    private readonly ITestOutputHelper _helper;

    public MyE2ETest(UnifiedBrowserFixture browserFixture, ITestOutputHelper helper)
    {
        _browserFixture = browserFixture;
        _helper = helper;
    }

    [Fact]
    public async Task Should_Work_With_Default_Framework()
    {
        var driver = await _browserFixture.GetDriverAsync();
        await driver.NavigateAsync("https://localhost:7072");
        
        var indexPage = new UnifiedIndexPage(driver, _helper);
        await indexPage.AddMessageAsync("Test message");
        
        Assert.True(await indexPage.HasMessageAsync("Test message"));
    }
}
```

### 2. Force Specific Framework

```csharp
public class PlaywrightSpecificTest : IDisposable
{
    private readonly UnifiedBrowserFixture _browserFixture;

    public PlaywrightSpecificTest()
    {
        var config = new E2ETestConfiguration
        {
            Framework = TestFramework.Playwright,
            Browser = BrowserType.Chrome,
            Headless = true
        };
        
        _browserFixture = new UnifiedBrowserFixture(config);
    }

    [Fact] 
    public async Task Should_Use_Playwright()
    {
        var driver = await _browserFixture.GetDriverAsync();
        // Test logic here...
    }

    public void Dispose() => _browserFixture?.Dispose();
}
```

### 3. Page Object Implementation

```csharp
public class MyPage : UnifiedPageObjectBase
{
    public MyPage(IBrowserDriver driver, ITestOutputHelper? helper = null) 
        : base(driver, helper) { }

    public async Task<IElementLocator> GetSubmitButtonAsync()
    {
        return await Driver.FindElementAsync(By.Id("submitBtn"));
    }

    public async Task SubmitFormAsync()
    {
        var button = await GetSubmitButtonAsync();
        await button.ClickAsync();
        
        // Wait for form submission to complete
        await WaitForConditionAsync(async () => 
        {
            var url = await Driver.GetUrlAsync();
            return url.Contains("success");
        });
    }
}
```

## 🚀 Running Tests

### Run with Default Framework (Selenium)

```bash
dotnet test src/RazorPagesProject.E2ETests/
```

### Run with Playwright via Environment Variable

```bash
E2E_FRAMEWORK=Playwright dotnet test src/RazorPagesProject.E2ETests/
```

### Run Specific Framework Tests

```bash
# Test only Selenium
dotnet test src/RazorPagesProject.E2ETests/ --filter "SeleniumSpecificTest"

# Test only Playwright  
dotnet test src/RazorPagesProject.E2ETests/ --filter "PlaywrightSpecificTest"
```

## 🔧 Setup Requirements

### Selenium Requirements

- **Edge WebDriver**: Automatically downloaded by Selenium
- **Chrome/Firefox**: Requires drivers in PATH or automatic download

### Playwright Requirements

```bash
# Install Playwright browsers
cd src/RazorPagesProject.E2ETests
playwright install
```

## 📊 Framework Comparison

| Feature | Selenium | Playwright |
|---------|----------|------------|
| **Speed** | Moderate | Faster |
| **Stability** | Good | Excellent |
| **Browser Support** | Chrome, Edge, Firefox, Safari | Chrome, Edge, Firefox |
| **Setup** | Simple | Requires browser installation |
| **Debugging** | DevTools available | Better debugging tools |
| **Wait Strategies** | Explicit waits | Built-in waiting |

## 🛠️ Best Practices

### 1. Use Unified Page Objects

Always use the unified page objects to ensure compatibility with both frameworks:

```csharp
// ✅ Good - Works with both frameworks
var page = new UnifiedIndexPage(driver, helper);

// ❌ Avoid - Framework-specific
var seleniumPage = new IndexPage(webDriver, helper);
```

### 2. Prefer Async Methods

Use async/await consistently:

```csharp
// ✅ Good
await page.AddMessageAsync("test");
var hasMessage = await page.HasMessageAsync("test");

// ❌ Avoid blocking calls
page.AddMessageAsync("test").Wait();
```

### 3. Use Configuration for Environment-Specific Settings

```csharp
// ✅ Good - Configurable
var baseUrl = _browserFixture.Configuration.BaseUrl;

// ❌ Avoid hardcoding
var baseUrl = "https://localhost:7072";
```

### 4. Handle Framework Differences Gracefully

The adapters handle most differences, but be aware of framework-specific behaviors:

```csharp
// Both frameworks handle this uniformly
await driver.ExecuteScriptAsync("arguments[0].click();", element.GetNativeElement());
```

## 🐛 Troubleshooting

### Common Issues

1. **Playwright browsers not installed**:
   ```bash
   playwright install
   ```

2. **Configuration not loading**:
   - Check appsettings.json exists
   - Verify environment variable names (E2E_ prefix)

3. **Element not found**:
   - Use explicit waits: `await driver.FindElementAsync(By.Id("element"), TimeSpan.FromSeconds(10))`
   - Check selector compatibility between frameworks

4. **Tests failing intermittently**:
   - Increase timeout values
   - Use proper wait strategies instead of Thread.Sleep

### Debugging

Enable verbose logging:

```csharp
_helper.WriteLine($"Testing with framework: {_browserFixture.Framework}");
_helper.WriteLine($"Current URL: {await driver.GetUrlAsync()}");
```

## 📈 Migration Guide

### From Existing Selenium Tests

1. Replace `IWebDriver` with `IBrowserDriver`
2. Replace `IWebElement` with `IElementLocator`  
3. Add async/await to method calls
4. Use `UnifiedBrowserFixture` instead of framework-specific fixtures
5. Update page objects to inherit from `UnifiedPageObjectBase`

### Example Migration

**Before (Selenium-only)**:
```csharp
public class OldTest : IClassFixture<EdgeFixture>
{
    private readonly IWebDriver _driver;
    
    public OldTest(EdgeFixture fixture)
    {
        _driver = fixture.Driver;
    }
    
    [Fact]
    public void TestSomething()
    {
        _driver.Navigate().GoToUrl("https://localhost:7072");
        var element = _driver.FindElement(By.Id("test"));
        element.Click();
    }
}
```

**After (Unified)**:
```csharp
public class NewTest : IClassFixture<UnifiedBrowserFixture>
{
    private readonly UnifiedBrowserFixture _fixture;
    
    public NewTest(UnifiedBrowserFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task TestSomething()
    {
        var driver = await _fixture.GetDriverAsync();
        await driver.NavigateAsync("https://localhost:7072");
        var element = await driver.FindElementAsync(By.Id("test"));
        await element.ClickAsync();
    }
}
```

This unified approach provides maximum flexibility while maintaining code reuse between testing frameworks.