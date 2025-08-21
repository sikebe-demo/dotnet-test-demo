using Xunit;
using RazorPagesProject.Services;
using RazorPagesProject.Models;

namespace RazorPagesProject.IntegrationTests.UnitTests;

public class UserAgentClassificationServiceTests
{
    private readonly UserAgentClassificationService _service = new();

    [Theory]
    [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15", DeviceType.Mobile)]
    [InlineData("Mozilla/5.0 (Linux; Android 11; SM-G991B) AppleWebKit/537.36", DeviceType.Mobile)]
    [InlineData("Mozilla/5.0 (Linux; Android 6.0; Mobile; rv:68.0) Gecko/68.0 Firefox/68.0", DeviceType.Mobile)]
    [InlineData("BlackBerry9700/5.0.0.862 Profile/MIDP-2.1 Configuration/CLDC-1.1 VendorID/331", DeviceType.Mobile)]
    [InlineData("Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/19.916; U; en) Presto/2.5.25", DeviceType.Mobile)]
    public void ClassifyUserAgent_MobileDevices_ReturnsMobile(string userAgent, DeviceType expected)
    {
        // Arrange, Act
        var result = _service.ClassifyUserAgent(userAgent);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (iPad; CPU OS 15_0 like Mac OS X) AppleWebKit/605.1.15", DeviceType.Tablet)]
    [InlineData("Mozilla/5.0 (Linux; Android 10; SM-T510) AppleWebKit/537.36", DeviceType.Tablet)]
    [InlineData("Mozilla/5.0 (Linux; Android 7.0; Nexus 9) AppleWebKit/537.36", DeviceType.Tablet)]
    [InlineData("Mozilla/5.0 (Linux; Android 4.4.2; Nexus 7) AppleWebKit/537.36", DeviceType.Tablet)]
    [InlineData("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; ARM; Trident/6.0; Touch; ARMBJS; Tablet PC 2.0)", DeviceType.Tablet)]
    public void ClassifyUserAgent_TabletDevices_ReturnsTablet(string userAgent, DeviceType expected)
    {
        // Arrange, Act
        var result = _service.ClassifyUserAgent(userAgent);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36", DeviceType.Desktop)]
    [InlineData("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36", DeviceType.Desktop)]
    [InlineData("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36", DeviceType.Desktop)]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:91.0) Gecko/20100101 Firefox/91.0", DeviceType.Desktop)]
    public void ClassifyUserAgent_DesktopDevices_ReturnsDesktop(string userAgent, DeviceType expected)
    {
        // Arrange, Act
        var result = _service.ClassifyUserAgent(userAgent);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ClassifyUserAgent_NullOrEmptyUserAgent_ReturnsDesktop(string? userAgent)
    {
        // Arrange, Act
        var result = _service.ClassifyUserAgent(userAgent);

        // Assert
        Assert.Equal(DeviceType.Desktop, result);
    }

    [Fact]
    public void ClassifyUserAgent_UnknownUserAgent_ReturnsDesktop()
    {
        // Arrange
        var userAgent = "CustomBot/1.0";

        // Act
        var result = _service.ClassifyUserAgent(userAgent);

        // Assert
        Assert.Equal(DeviceType.Desktop, result);
    }

    [Fact]
    public void ClassifyUserAgent_TabletTakesPrecedenceOverMobile()
    {
        // Arrange - User agent that contains both mobile and tablet indicators
        var userAgent = "Mozilla/5.0 (Linux; Android 10; SM-T510) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.120 Mobile Safari/537.36";

        // Act
        var result = _service.ClassifyUserAgent(userAgent);

        // Assert
        Assert.Equal(DeviceType.Tablet, result);
    }
}