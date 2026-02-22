using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Middleware;

namespace RazorPagesProject.Pages;

public class DeviceAnalyticsModel : PageModel
{
    public DeviceStatistics Statistics { get; private set; } = new();

    public void OnGet()
    {
        Statistics = DeviceAnalyticsMiddleware.GetStatistics();
    }
}
