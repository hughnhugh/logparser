using MantelLogParser.Parsing;

namespace MantelLogParser.Tests.Parsing;

public class LogParserTests
{
    [Fact]
    public void ParseLines_ParsesValidLine()
    {
        var parser = new LogParser();
        var line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /intranet-analytics/ HTTP/1.1\" 200 3574 \"-\" \"Mozilla/5.0\"";

        var result = parser.ParseLines([line]).Single();

        Assert.Equal("177.71.128.21", result.IpAddress);
        Assert.Equal("/intranet-analytics/", result.Url);
    }

    [Fact]
    public void ParseLines_IgnoresMalformedLine()
    {
        var parser = new LogParser();

        var result = parser.ParseLines(["this is not a log line"]);

        Assert.Empty(result);
    }

    [Fact]
    public void ParseLines_AllowsTrailingTokens()
    {
        var parser = new LogParser();
        var line = "72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] \"GET / HTTP/1.1\" 200 3574 \"-\" \"Mozilla/5.0\" junk extra";

        var result = parser.ParseLines([line]).Single();

        Assert.Equal("72.44.32.10", result.IpAddress);
        Assert.Equal("/", result.Url);
    }
}
