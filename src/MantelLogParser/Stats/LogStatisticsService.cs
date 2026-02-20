using MantelLogParser.Core;

namespace MantelLogParser.Stats;

public sealed class LogStatisticsService
{
    public Report BuildReport(IEnumerable<LogEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var snapshot = entries.ToList();

        return new Report
        {
            UniqueIpAddressCount = snapshot
                .Select(e => e.IpAddress)
                .Distinct(StringComparer.Ordinal)
                .Count(),
            TopVisitedUrls = snapshot
                .GroupBy(e => e.Url, StringComparer.Ordinal)
                .Select(group => new CountResult(group.Key, group.Count()))
                .OrderByDescending(result => result.Count)
                .ThenBy(result => result.Value, StringComparer.Ordinal)
                .Take(3)
                .ToList(),
            TopActiveIpAddresses = snapshot
                .GroupBy(e => e.IpAddress, StringComparer.Ordinal)
                .Select(group => new CountResult(group.Key, group.Count()))
                .OrderByDescending(result => result.Count)
                .ThenBy(result => result.Value, StringComparer.Ordinal)
                .Take(3)
                .ToList(),
        };
    }
}
