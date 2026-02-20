namespace MantelLogParser.Core;

public sealed record CountResult(string Value, int Count);

public sealed class Report
{
    public int UniqueIpAddressCount { get; init; }

    public IReadOnlyList<CountResult> TopVisitedUrls { get; init; } = [];

    public IReadOnlyList<CountResult> TopActiveIpAddresses { get; init; } = [];
}
