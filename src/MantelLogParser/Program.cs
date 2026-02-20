using MantelLogParser.Parsing;
using MantelLogParser.Stats;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: MantelLogParser <path-to-log-file>");
    return 1;
}

var logFilePath = args[0];
if (!File.Exists(logFilePath))
{
    Console.Error.WriteLine($"Log file not found: {logFilePath}");
    return 1;
}

var parser = new LogParser();
var statisticsService = new LogStatisticsService();

var entries = parser.ParseLines(File.ReadLines(logFilePath));
var report = statisticsService.BuildReport(entries);

Console.WriteLine($"Unique IP addresses: {report.UniqueIpAddressCount}");
Console.WriteLine();
Console.WriteLine("Top 3 most visited URLs:");
foreach (var item in report.TopVisitedUrls)
{
    Console.WriteLine($"- {item.Value} ({item.Count})");
}

Console.WriteLine();
Console.WriteLine("Top 3 most active IP addresses:");
foreach (var item in report.TopActiveIpAddresses)
{
    Console.WriteLine($"- {item.Value} ({item.Count})");
}

return 0;
