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

    [Fact]
    public void BuildReport_WhenEntriesIsNull_Throws()
    {
        var service = new LogStatisticsService();
        Assert.Throws<ArgumentNullException>(() => service.BuildReport(null!));
    }

    [Fact]
    public void BuildReport_WhenEmpty_ReturnsZeroAndEmptyLists()
    {
        var service = new LogStatisticsService();

        var report = service.BuildReport(Array.Empty<LogEntry>());

        Assert.Equal(0, report.UniqueIpAddressCount);
        Assert.Empty(report.TopVisitedUrls);
        Assert.Empty(report.TopActiveIpAddresses);
    }

    [Fact]
    public void BuildReport_TopVisitedUrls_TieBreaksByValueAscending_WhenCountsEqual()
    {
        var service = new LogStatisticsService();

        // All URLs occur once. Expect lexicographically smallest 3.
        var entries = new[]
        {
            new LogEntry("1.1.1.1", "/z"),
            new LogEntry("1.1.1.1", "/x"),
            new LogEntry("1.1.1.1", "/y"),
            new LogEntry("1.1.1.2", "/b"),
            new LogEntry("1.1.1.3", "/a"),
            new LogEntry("1.1.1.4", "/c"),
        };

        var report = service.BuildReport(entries);

        Assert.Equal(
            new[]
            {
                new CountResult("/a", 1),
                new CountResult("/b", 1),
                new CountResult("/c", 1),
            },
            report.TopVisitedUrls);
    }

    [Fact]
    public void BuildReport_TopActiveIps_TieBreaksByValueAscending_WhenCountsEqual()
    {
        var service = new LogStatisticsService();

        // Each IP occurs twice. Expect smallest 3 IP strings ordinal.
        var entries = new[]
        {
            new LogEntry("10.0.0.9", "/x"),
            new LogEntry("10.0.0.9", "/y"),

            new LogEntry("10.0.0.2", "/x"),
            new LogEntry("10.0.0.2", "/y"),

            new LogEntry("10.0.0.10", "/x"),
            new LogEntry("10.0.0.10", "/y"),

            new LogEntry("10.0.0.1", "/x"),
            new LogEntry("10.0.0.1", "/y"),
        };

        var report = service.BuildReport(entries);

        Assert.Equal(
            new[]
            {
                new CountResult("10.0.0.1", 2),
                new CountResult("10.0.0.10", 2),
                new CountResult("10.0.0.2", 2),
            },
            report.TopActiveIpAddresses);
    }

    [Fact]
    public void BuildReport_ReturnsAtMostThreeItems_EvenWhenMoreExist()
    {
        var service = new LogStatisticsService();

        var entries = new[]
        {
            new LogEntry("1", "/a"),
            new LogEntry("2", "/b"),
            new LogEntry("3", "/c"),
            new LogEntry("4", "/d"),
            new LogEntry("5", "/e"),
        };

        var report = service.BuildReport(entries);

        Assert.Equal(3, report.TopVisitedUrls.Count);
        Assert.Equal(3, report.TopActiveIpAddresses.Count);
    }

    [Fact]
    public void BuildReport_UniqueIpCount_UsesOrdinalComparison()
    {
        var service = new LogStatisticsService();

        // Ordinal distinct => case-sensitive.
        var entries = new[]
        {
            new LogEntry("ABC", "/a"),
            new LogEntry("abc", "/a"),
        };

        var report = service.BuildReport(entries);

        Assert.Equal(2, report.UniqueIpAddressCount);
    }
}
