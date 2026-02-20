namespace MantelLogParser.Core;

public sealed record LogEntry
{
    public LogEntry(
        string ipAddress,
        string? ident,
        string? authUser,
        DateTimeOffset timestamp,
        string method,
        string requestTarget,
        string httpVersion,
        int statusCode,
        long? bytes,
        string? referer,
        string? userAgent,
        string? trailing)
    {
        IpAddress = ipAddress;
        Ident = ident;
        AuthUser = authUser;
        Timestamp = timestamp;
        Method = method;
        RequestTarget = requestTarget;
        HttpVersion = httpVersion;
        StatusCode = statusCode;
        Bytes = bytes;
        Referer = referer;
        UserAgent = userAgent;
        Trailing = trailing;
    }

    public LogEntry(string ipAddress, string url)
        : this(
            ipAddress,
            ident: null,
            authUser: null,
            timestamp: DateTimeOffset.MinValue,
            method: string.Empty,
            requestTarget: url,
            httpVersion: string.Empty,
            statusCode: 0,
            bytes: null,
            referer: null,
            userAgent: null,
            trailing: null)
    {
    }

    public string IpAddress { get; }

    public string? Ident { get; }

    public string? AuthUser { get; }

    public DateTimeOffset Timestamp { get; }

    public string Method { get; }

    public string RequestTarget { get; }

    public string HttpVersion { get; }

    public int StatusCode { get; }

    public long? Bytes { get; }

    public string? Referer { get; }

    public string? UserAgent { get; }

    public string? Trailing { get; }

    public string Url => RequestTarget;
}
