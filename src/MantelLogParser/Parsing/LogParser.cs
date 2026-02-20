using System.Text.RegularExpressions;
using MantelLogParser.Core;

namespace MantelLogParser.Parsing;

public sealed partial class LogParser
{
    [GeneratedRegex("^(?<ip>\\S+)\\s+\\S+\\s+\\S+\\s+\\[[^\\]]+\\]\\s+\"[A-Z]+\\s+(?<url>\\S+)\\s+HTTP/[0-9.]+\"\\s+\\d{3}\\s+\\S+\\s+\"[^\"]*\"\\s+\"[^\"]*\"(?:\\s+.*)?$")]
    private static partial Regex LogLineRegex();

    public IEnumerable<LogEntry> ParseLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var match = LogLineRegex().Match(line);
            if (!match.Success)
            {
                continue;
            }

            yield return new LogEntry(
                match.Groups["ip"].Value,
                match.Groups["url"].Value);
        }
    }
}
