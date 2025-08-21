using RazorPagesProject.Models;

namespace RazorPagesProject.Services;

public interface IDeviceCountersService
{
    void IncrementCounter(DeviceType deviceType);
    DeviceUsageSummary GetCumulativeSummary();
    DeviceUsageSummary GetRollingHourSummary();
}