using Xunit;
using RazorPagesProject.Services;
using RazorPagesProject.Models;

namespace RazorPagesProject.IntegrationTests.UnitTests;

public class DeviceCountersServiceTests
{
    [Fact]
    public void IncrementCounter_SingleDevice_UpdatesCountersCorrectly()
    {
        // Arrange
        var service = new DeviceCountersService();

        // Act
        service.IncrementCounter(DeviceType.Mobile);
        service.IncrementCounter(DeviceType.Mobile);
        service.IncrementCounter(DeviceType.Desktop);

        // Assert
        var cumulativeSummary = service.GetCumulativeSummary();
        Assert.Equal(-1, cumulativeSummary.WindowMinutes); // Cumulative indicator
        Assert.Equal(2, cumulativeSummary.Mobile);
        Assert.Equal(0, cumulativeSummary.Tablet);
        Assert.Equal(1, cumulativeSummary.Desktop);

        var rollingSummary = service.GetRollingHourSummary();
        Assert.Equal(60, rollingSummary.WindowMinutes);
        Assert.Equal(2, rollingSummary.Mobile);
        Assert.Equal(0, rollingSummary.Tablet);
        Assert.Equal(1, rollingSummary.Desktop);
    }

    [Fact]
    public void GetCumulativeSummary_EmptyCounters_ReturnsZeros()
    {
        // Arrange
        var service = new DeviceCountersService();

        // Act
        var summary = service.GetCumulativeSummary();

        // Assert
        Assert.Equal(-1, summary.WindowMinutes);
        Assert.Equal(0, summary.Mobile);
        Assert.Equal(0, summary.Tablet);
        Assert.Equal(0, summary.Desktop);
    }

    [Fact]
    public void GetRollingHourSummary_EmptyCounters_ReturnsZeros()
    {
        // Arrange
        var service = new DeviceCountersService();

        // Act
        var summary = service.GetRollingHourSummary();

        // Assert
        Assert.Equal(60, summary.WindowMinutes);
        Assert.Equal(0, summary.Mobile);
        Assert.Equal(0, summary.Tablet);
        Assert.Equal(0, summary.Desktop);
    }

    [Fact]
    public void IncrementCounter_AllDeviceTypes_UpdatesCorrectly()
    {
        // Arrange
        var service = new DeviceCountersService();

        // Act
        service.IncrementCounter(DeviceType.Mobile);
        service.IncrementCounter(DeviceType.Tablet);
        service.IncrementCounter(DeviceType.Desktop);
        service.IncrementCounter(DeviceType.Mobile);

        // Assert
        var cumulativeSummary = service.GetCumulativeSummary();
        Assert.Equal(2, cumulativeSummary.Mobile);
        Assert.Equal(1, cumulativeSummary.Tablet);
        Assert.Equal(1, cumulativeSummary.Desktop);

        var rollingSummary = service.GetRollingHourSummary();
        Assert.Equal(2, rollingSummary.Mobile);
        Assert.Equal(1, rollingSummary.Tablet);
        Assert.Equal(1, rollingSummary.Desktop);
    }
}