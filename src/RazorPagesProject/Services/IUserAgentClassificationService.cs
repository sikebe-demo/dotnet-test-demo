using RazorPagesProject.Models;

namespace RazorPagesProject.Services;

public interface IUserAgentClassificationService
{
    DeviceType ClassifyUserAgent(string? userAgent);
}