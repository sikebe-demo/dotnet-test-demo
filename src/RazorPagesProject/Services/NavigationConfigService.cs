namespace RazorPagesProject.Services;

/// <summary>
/// Service to provide navigation configuration based on analytics
/// </summary>
public interface INavigationConfigService
{
    /// <summary>
    /// Get the Bootstrap navbar CSS class based on mobile usage analytics
    /// </summary>
    Task<string> GetNavbarExpandClassAsync();
    
    /// <summary>
    /// Check if hamburger menu is currently enabled
    /// </summary>
    Task<bool> IsHamburgerMenuEnabledAsync();
}

public class NavigationConfigService : INavigationConfigService
{
    private readonly IDeviceAnalyticsService _analyticsService;
    private readonly ILogger<NavigationConfigService> _logger;
    private readonly IConfiguration _configuration;

    public NavigationConfigService(
        IDeviceAnalyticsService analyticsService,
        ILogger<NavigationConfigService> logger,
        IConfiguration configuration)
    {
        _analyticsService = analyticsService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> GetNavbarExpandClassAsync()
    {
        var isEnabled = await IsHamburgerMenuEnabledAsync();
        
        if (isEnabled)
        {
            // Use navbar-expand-md (768px breakpoint) as closest to 640px requirement
            // Custom CSS will override this to exactly 640px
            var navbarClass = "navbar-expand-md navbar-hamburger-enabled";
            _logger.LogDebug("Hamburger menu enabled, using class: {NavbarClass}", navbarClass);
            return navbarClass;
        }
        
        // Keep default behavior - expand on small screens and above (576px+)
        var defaultClass = "navbar-expand-sm";
        _logger.LogDebug("Hamburger menu disabled, using default class: {NavbarClass}", defaultClass);
        return defaultClass;
    }

    public async Task<bool> IsHamburgerMenuEnabledAsync()
    {
        try
        {
            return await _analyticsService.ShouldEnableHamburgerMenuAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking hamburger menu status, defaulting to disabled");
            return false;
        }
    }
}