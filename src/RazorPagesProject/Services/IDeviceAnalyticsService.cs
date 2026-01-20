using RazorPagesProject.Models;

namespace RazorPagesProject.Services;

public interface IDeviceAnalyticsService
{
    void RecordRequest(string userAgent);
    DeviceAnalyticsSummary GetSummary();
    DeviceType ClassifyDevice(string userAgent);
}
