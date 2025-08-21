using RazorPagesProject.Services;

namespace RazorPagesProject.HostedServices;

public class DeviceUsageLoggingServiceDemo : BackgroundService
{
    private readonly ILogger<DeviceUsageLoggingServiceDemo> _logger;
    private readonly IDeviceCountersService _countersService;
    private readonly TimeSpan _loggingInterval = TimeSpan.FromSeconds(10); // Demo: shortened interval

    public DeviceUsageLoggingServiceDemo(
        ILogger<DeviceUsageLoggingServiceDemo> logger,
        IDeviceCountersService countersService)
    {
        _logger = logger;
        _countersService = countersService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Device usage logging demo service started with 10-second interval");

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
        var finalCumulativeSummary = _countersService.GetCumulativeSummary();
        var finalRollingSummary = _countersService.GetRollingHourSummary();

        _logger.LogInformation("DEMO Final DeviceUsageSummary - Cumulative since startup: {{ windowMinutes={WindowMinutes}, mobile={Mobile}, tablet={Tablet}, desktop={Desktop} }}", 
            finalCumulativeSummary.WindowMinutes, 
            finalCumulativeSummary.Mobile, 
            finalCumulativeSummary.Tablet, 
            finalCumulativeSummary.Desktop);

        _logger.LogInformation("DEMO Final DeviceUsageSummary - Rolling hour: {{ windowMinutes={WindowMinutes}, mobile={Mobile}, tablet={Tablet}, desktop={Desktop} }}", 
            finalRollingSummary.WindowMinutes, 
            finalRollingSummary.Mobile, 
            finalRollingSummary.Tablet, 
            finalRollingSummary.Desktop);

        _logger.LogInformation("Device usage logging demo service stopped");

        await base.StopAsync(cancellationToken);
    }
}