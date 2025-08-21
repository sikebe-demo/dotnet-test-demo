namespace RazorPagesProject.Models;

public class DeviceUsageSummary
{
    public int WindowMinutes { get; set; }
    public int Mobile { get; set; }
    public int Tablet { get; set; }
    public int Desktop { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}