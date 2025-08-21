using System.Collections.Concurrent;
using RazorPagesProject.Models;

namespace RazorPagesProject.Services;

public class DeviceCountersService : IDeviceCountersService
{
    private readonly object _lock = new();
    private readonly ConcurrentDictionary<DeviceType, long> _cumulativeCounters = new();
    private readonly List<(DateTime timestamp, DeviceType deviceType)> _rollingEntries = new();

    public void IncrementCounter(DeviceType deviceType)
    {
        _cumulativeCounters.AddOrUpdate(deviceType, 1, (_, count) => count + 1);

        lock (_lock)
        {
            _rollingEntries.Add((DateTime.UtcNow, deviceType));
            
            // Clean up entries older than 1 hour
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            _rollingEntries.RemoveAll(entry => entry.timestamp < oneHourAgo);
        }
    }

    public DeviceUsageSummary GetCumulativeSummary()
    {
        return new DeviceUsageSummary
        {
            WindowMinutes = -1, // Indicates cumulative (since start)
            Mobile = (int)_cumulativeCounters.GetValueOrDefault(DeviceType.Mobile, 0),
            Tablet = (int)_cumulativeCounters.GetValueOrDefault(DeviceType.Tablet, 0),
            Desktop = (int)_cumulativeCounters.GetValueOrDefault(DeviceType.Desktop, 0)
        };
    }

    public DeviceUsageSummary GetRollingHourSummary()
    {
        lock (_lock)
        {
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            var recentEntries = _rollingEntries.Where(entry => entry.timestamp >= oneHourAgo).ToList();

            return new DeviceUsageSummary
            {
                WindowMinutes = 60,
                Mobile = recentEntries.Count(entry => entry.deviceType == DeviceType.Mobile),
                Tablet = recentEntries.Count(entry => entry.deviceType == DeviceType.Tablet),
                Desktop = recentEntries.Count(entry => entry.deviceType == DeviceType.Desktop)
            };
        }
    }
}