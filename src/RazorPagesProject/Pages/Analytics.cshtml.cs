using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Middleware;

namespace RazorPagesProject.Pages;

public class AnalyticsModel : PageModel
{
    public Dictionary<string, AccessMetrics> Analytics { get; set; } = new();
    public double MobilePercentage { get; set; }

    public void OnGet()
    {
        Analytics = AccessAnalyticsMiddleware.GetAnalytics();
        MobilePercentage = AccessAnalyticsMiddleware.GetMobileUsagePercentage();
    }
}