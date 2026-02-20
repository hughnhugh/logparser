using System.Globalization;
using System.Text.RegularExpressions;
using MantelLogParser.Core;

namespace MantelLogParser.Parsing;

public sealed partial class LogParser
{
    [GeneratedRegex("^(?<ip>\\S+)\\s+(?<ident>\\S+)\\s+(?<authuser>\\S+)\\s+\\[(?<timestamp>[^\\]]+)\\]\\s+\"(?<request>[^\"]*)\"\\s+(?<status>\\d{3})\\s+(?<bytes>\\S+)\\s+\"(?<referer>[^\"]*)\"\\s+\"(?<useragent>[^\"]*)\"(?<trailing>(?:\\s+.*)?)$")]
    private static partial Regex LogLineRegex();

    [GeneratedRegex("^(?<method>\\S+)\\s+(?<target>\\S+)\\s+HTTP/(?<version>\\S+)$")]
    private static partial Regex RequestLineRegex();

    public bool TryParse(string line, out LogEntry entry)
    {
        entry = null!;

        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        var match = LogLineRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        if (!DateTimeOffset.TryParseExact(
                match.Groups["timestamp"].Value,
                "dd/MMM/yyyy:HH:mm:ss zzz",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var timestamp))
        {
            return false;
        }

        var requestMatch = RequestLineRegex().Match(match.Groups["request"].Value);
        if (!requestMatch.Success)
        {
            return false;
        }

        if (!int.TryParse(match.Groups["status"].Value, out var statusCode))
        {
            return false;
        }

        if (!TryParseBytes(match.Groups["bytes"].Value, out var bytes))
        {
            return false;
        }

        entry = new LogEntry(
            ipAddress: match.Groups["ip"].Value,
            ident: ToNullableField(match.Groups["ident"].Value),
            authUser: ToNullableField(match.Groups["authuser"].Value),
            timestamp: timestamp,
            method: requestMatch.Groups["method"].Value,
            requestTarget: requestMatch.Groups["target"].Value,
            httpVersion: requestMatch.Groups["version"].Value,
            statusCode: statusCode,
            bytes: bytes,
            referer: ToNullableField(match.Groups["referer"].Value),
            userAgent: ToNullableField(match.Groups["useragent"].Value),
            trailing: ToNullableTrailing(match.Groups["trailing"].Value));

        return true;
    }

    public IEnumerable<LogEntry> ParseLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        foreach (var line in lines)
        {
            if (TryParse(line, out var entry))
            {
                yield return entry;
            }
        }
    }

    private static bool TryParseBytes(string rawValue, out long? bytes)
    {
        if (rawValue == "-")
        {
            bytes = null;
            return true;
        }

        if (long.TryParse(rawValue, out var parsedBytes))
        {
            bytes = parsedBytes;
            return true;
        }

        bytes = null;
        return false;
    }

    private static string? ToNullableField(string value) => value == "-" ? null : value;

    private static string? ToNullableTrailing(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.TrimStart();
    }
}
