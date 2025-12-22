using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Models;
using RazorPagesProject.Services;

namespace RazorPagesProject.Pages;

public class AnalyticsModel : PageModel
{
    private readonly IDeviceAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsModel> _logger;

    public DeviceAnalyticsSummary Summary { get; set; } = new();

    public AnalyticsModel(IDeviceAnalyticsService analyticsService, ILogger<AnalyticsModel> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    public void OnGet()
    {
        try
        {
            Summary = _analyticsService.GetSummary();
            _logger.LogInformation("Analytics page accessed. Total requests: {TotalRequests}", Summary.TotalRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics summary");
            Summary = new DeviceAnalyticsSummary();
        }
    }
}
