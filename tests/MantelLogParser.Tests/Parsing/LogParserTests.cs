using MantelLogParser.Parsing;

namespace MantelLogParser.Tests.Parsing;

public class LogParserTests
{
    [Fact]
    public void TryParse_ParsesStandardLine()
    {
        var parser = new LogParser();
        var line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /intranet-analytics/ HTTP/1.1\" 200 3574 \"-\" \"Mozilla/5.0\"";

        var success = parser.TryParse(line, out var entry);

        Assert.True(success);
        Assert.Equal("177.71.128.21", entry.IpAddress);
        Assert.Null(entry.Ident);
        Assert.Null(entry.AuthUser);
        Assert.Equal(new DateTimeOffset(2018, 7, 10, 22, 21, 28, TimeSpan.FromHours(2)), entry.Timestamp);
        Assert.Equal("GET", entry.Method);
        Assert.Equal("/intranet-analytics/", entry.RequestTarget);
        Assert.Equal("1.1", entry.HttpVersion);
        Assert.Equal(200, entry.StatusCode);
        Assert.Equal(3574, entry.Bytes);
        Assert.Null(entry.Referer);
        Assert.Equal("Mozilla/5.0", entry.UserAgent);
        Assert.Null(entry.Trailing);
    }

    [Fact]
    public void TryParse_ParsesAbsoluteUrlRequestTarget()
    {
        var parser = new LogParser();
        var line = "72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] \"GET https://example.com/home?q=a HTTP/1.1\" 301 123 \"https://ref.example/\" \"curl/8.6.0\"";

        var success = parser.TryParse(line, out var entry);

        Assert.True(success);
        Assert.Equal("https://example.com/home?q=a", entry.RequestTarget);
        Assert.Equal("https://example.com/home?q=a", entry.Url);
        Assert.Equal("https://ref.example/", entry.Referer);
    }

    [Fact]
    public void TryParse_ParsesAuthUserWhenPresent()
    {
        var parser = new LogParser();
        var line = "72.44.32.10 - admin [09/Jul/2018:15:48:07 +0200] \"POST /secure HTTP/2\" 201 - \"-\" \"Mozilla/5.0\"";

        var success = parser.TryParse(line, out var entry);

        Assert.True(success);
        Assert.Null(entry.Ident);
        Assert.Equal("admin", entry.AuthUser);
        Assert.Equal("POST", entry.Method);
        Assert.Equal("2", entry.HttpVersion);
        Assert.Null(entry.Bytes);
    }

    [Fact]
    public void TryParse_CapturesTrailingTokens()
    {
        var parser = new LogParser();
        var line = "72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] \"GET / HTTP/1.1\" 200 3574 \"-\" \"Mozilla/5.0\" junk extra";

        var success = parser.TryParse(line, out var entry);

        Assert.True(success);
        Assert.Equal("junk extra", entry.Trailing);
    }

    [Fact]
    public void ParseLines_IgnoresMalformedAndBlankLines()
    {
        var parser = new LogParser();
        var line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /intranet-analytics/ HTTP/1.1\" 200 3574 \"-\" \"Mozilla/5.0\"";

        var result = parser.ParseLines(["", "this is not a log line", line]).ToList();

        Assert.Single(result);
        Assert.Equal("177.71.128.21", result[0].IpAddress);
        Assert.Equal("/intranet-analytics/", result[0].RequestTarget);
    }
}
