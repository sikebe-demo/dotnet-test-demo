using System.Text.RegularExpressions;
using RazorPagesProject.Models;

namespace RazorPagesProject.Services;

public partial class UserAgentClassificationService : IUserAgentClassificationService
{
    private static readonly Regex MobilePattern = GenerateMobileRegex();
    private static readonly Regex TabletPattern = GenerateTabletRegex();

    public DeviceType ClassifyUserAgent(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return DeviceType.Desktop;
        }

        // Check for tablet first (more specific)
        if (TabletPattern.IsMatch(userAgent))
        {
            return DeviceType.Tablet;
        }

        // Then check for mobile
        if (MobilePattern.IsMatch(userAgent))
        {
            return DeviceType.Mobile;
        }

        // Default to desktop
        return DeviceType.Desktop;
    }

    [GeneratedRegex(@"(?i)\b(iPad|tablet|kindle|nook|nexus\s7|nexus\s9|nexus\s10|galaxy\stab|xoom|tab\s|playbook|sm-t\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex GenerateTabletRegex();

    [GeneratedRegex(@"(?i)\b(mobile|iphone|ipod|android|blackberry|opera\smini|iemobile|windows\sphone|palm|smartphone|j2me|midp|pocket|mobile\sphone|pda)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex GenerateMobileRegex();
}