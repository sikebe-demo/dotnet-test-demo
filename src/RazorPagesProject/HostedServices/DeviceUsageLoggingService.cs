using RazorPagesProject.Services;

namespace RazorPagesProject.HostedServices;

public class DeviceUsageLoggingService : BackgroundService
{
    private readonly ILogger<DeviceUsageLoggingService> _logger;
    private readonly IDeviceCountersService _countersService;
    private readonly TimeSpan _loggingInterval = TimeSpan.FromMinutes(5);

    public DeviceUsageLoggingService(
        ILogger<DeviceUsageLoggingService> logger,
        IDeviceCountersService countersService)
    {
        _logger = logger;
        _countersService = countersService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Device usage logging service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_loggingInterval, stoppingToken);
                
                var rollingSummary = _countersService.GetRollingHourSummary();
                
                _logger.LogInformation("DeviceUsageSummary {{ windowMinutes={WindowMinutes}, mobile={Mobile}, tablet={Tablet}, desktop={Desktop} }}", 
                    rollingSummary.WindowMinutes, 
                    rollingSummary.Mobile, 
                    rollingSummary.Tablet, 
                    rollingSummary.Desktop);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging device usage statistics");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Log final summary on shutdown
        var finalCumulativeSummary = _countersService.GetCumulativeSummary();
        var finalRollingSummary = _countersService.GetRollingHourSummary();

        _logger.LogInformation("Final DeviceUsageSummary - Cumulative since startup: {{ windowMinutes={WindowMinutes}, mobile={Mobile}, tablet={Tablet}, desktop={Desktop} }}", 
            finalCumulativeSummary.WindowMinutes, 
            finalCumulativeSummary.Mobile, 
            finalCumulativeSummary.Tablet, 
            finalCumulativeSummary.Desktop);

        _logger.LogInformation("Final DeviceUsageSummary - Rolling hour: {{ windowMinutes={WindowMinutes}, mobile={Mobile}, tablet={Tablet}, desktop={Desktop} }}", 
            finalRollingSummary.WindowMinutes, 
            finalRollingSummary.Mobile, 
            finalRollingSummary.Tablet, 
            finalRollingSummary.Desktop);

        _logger.LogInformation("Device usage logging service stopped");

        await base.StopAsync(cancellationToken);
    }
}