# Device Analytics Feature

## Overview
This feature provides anonymous device analytics collection to support data-driven decision making for responsive design implementation.

## Purpose
To collect real-world usage data to determine whether implementing a mobile hamburger menu (for 375px and below) provides sufficient value based on actual mobile user traffic.

## Implementation

### Components

1. **DeviceAnalyticsMiddleware** (`Middleware/DeviceAnalyticsMiddleware.cs`)
   - Analyzes UserAgent headers to classify device types (Mobile, Tablet, Desktop)
   - Maintains thread-safe in-memory statistics
   - No PII collection - only aggregate counts

2. **Device Analytics Page** (`Pages/DeviceAnalytics.cshtml`)
   - Displays real-time statistics dashboard
   - Shows device type breakdown with percentages
   - Provides decision criteria guidance
   - Includes privacy notice

### UserAgent Classification Logic

- **Mobile**: Devices with "mobile", "iphone", or "ipod" in UserAgent, or Android devices with "mobile"
- **Tablet**: Devices with "ipad", "tablet", "kindle", or Android devices without "mobile"
- **Desktop**: All other devices (default)

### Decision Criteria

Based on collected data over approximately 1 week:

- **Mobile ≥ 30%**: Implement responsive hamburger menu
- **Mobile < 30%**: Defer implementation and prioritize other tasks

## Usage

1. The middleware automatically collects data from all page requests
2. Access the analytics dashboard at `/DeviceAnalytics`
3. Monitor the mobile percentage over time
4. Make implementation decision based on the 30% threshold

## Privacy

- Only device type information is collected from UserAgent headers
- No personally identifiable information (PII) is stored
- All data is aggregated anonymously
- Statistics are stored in-memory only

## Testing

Integration tests are provided in `RazorPagesProject.IntegrationTests/IntegrationTests/`:
- `DeviceAnalyticsTests.cs` - Tests UserAgent classification logic
- `DeviceAnalyticsPageTests.cs` - Tests the analytics page endpoint

## Future Implementation

If mobile traffic exceeds 30%, the next phase will include:
- Hamburger menu for screens ≤ 375px
- Responsive navigation improvements
- Accessibility-focused implementation
- Pure CSS solution (no Tailwind dependency)

## Technical Details

- **Thread Safety**: Uses lock-based synchronization for statistics updates
- **Performance**: Minimal overhead - simple string operations per request
- **Scalability**: In-memory storage suitable for moderate traffic; consider persistent storage for high-traffic scenarios

## Maintenance

To reset statistics (for testing or new collection period):
```csharp
DeviceAnalyticsMiddleware.ResetStatistics();
```
