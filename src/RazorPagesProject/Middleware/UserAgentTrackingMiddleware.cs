using RazorPagesProject.Services;

namespace RazorPagesProject.Middleware;

public class UserAgentTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserAgentClassificationService _classificationService;
    private readonly IDeviceCountersService _countersService;

    public UserAgentTrackingMiddleware(
        RequestDelegate next,
        IUserAgentClassificationService classificationService,
        IDeviceCountersService countersService)
    {
        _next = next;
        _classificationService = classificationService;
        _countersService = countersService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract and classify User-Agent
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var deviceType = _classificationService.ClassifyUserAgent(userAgent);
        
        // Increment counter
        _countersService.IncrementCounter(deviceType);

        // Continue with the request pipeline
        await _next(context);
    }
}