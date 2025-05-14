using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace RazorPagesProject.E2ETests.Fixtures;

public class EdgeFixture : BrowserFixture
{
    protected override IWebDriver CreateDriver()
    {
        var opts = new EdgeOptions();

        // Ignore self-signed certificate warnings
        opts.AddArgument("--ignore-certificate-errors");

        // 「...が次の許可を求めています」ダイアログを非表示
        opts.AddArgument("--disable-notifications");

        opts.AddArguments("-inprivate");

        // Workaround for https://github.com/SeleniumHQ/selenium/issues/15340
        if (Environment.GetEnvironmentVariable("CI") != "true")
        {
            opts.AddArgument("--edge-skip-compat-layer-relaunch");
        }

        // Comment this out if you want to watch or interact with the browser (e.g. for debugging)
        if (!Debugger.IsAttached)
        {
            opts.AddArgument("headless");
        }

        var driver = new EdgeDriver(EdgeDriverService.CreateDefaultService(), opts, TimeSpan.FromSeconds(60));
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        return driver;
    }
}
