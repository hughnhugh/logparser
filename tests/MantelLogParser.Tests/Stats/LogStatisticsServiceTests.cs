using MantelLogParser.Core;
using MantelLogParser.Stats;

namespace MantelLogParser.Tests.Stats;

public class LogStatisticsServiceTests
{
    [Fact]
    public void BuildReport_ComputesExpectedMetrics()
    {
        var service = new LogStatisticsService();
        var entries = new[]
        {
            new LogEntry("10.0.0.1", "/a"),
            new LogEntry("10.0.0.2", "/a"),
            new LogEntry("10.0.0.1", "/b"),
            new LogEntry("10.0.0.3", "/c"),
            new LogEntry("10.0.0.1", "/d"),
            new LogEntry("10.0.0.2", "/b"),
            new LogEntry("10.0.0.4", "/z"),
        };

        var report = service.BuildReport(entries);

        Assert.Equal(4, report.UniqueIpAddressCount);
        Assert.Equal(
            [
                new CountResult("/a", 2),
                new CountResult("/b", 2),
                new CountResult("/c", 1),
            ],
            report.TopVisitedUrls);
        Assert.Equal(
            [
                new CountResult("10.0.0.1", 3),
                new CountResult("10.0.0.2", 2),
                new CountResult("10.0.0.3", 1),
            ],
            report.TopActiveIpAddresses);
    }
}
