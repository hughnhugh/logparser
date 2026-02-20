# Mantel Log Parser

A small .NET 8 console application that parses HTTP access logs and reports:

- Number of unique IP addresses
- Top 3 most visited URLs
- Top 3 most active IP addresses

## Requirements

- .NET 8 SDK

## Run

```bash
dotnet run --project src/MantelLogParser -- path/to/log-file.log
dotnet run --project src/MantelLogParser -- programming-task-example-data.log
```

## Test

```bash
dotnet test
```

## Assumptions

- Only lines matching the expected access log shape are parsed.
- Malformed lines are ignored.
- Extra trailing tokens at the end of a valid log line are tolerated.
- URL is taken directly from the request target (no normalization).
- Top lists are sorted by count descending, then value ascending for deterministic ties.
- Errors and redirects (3xx, 4xx, 5xx) are considered valid URLs and IP addresses for counting purposes.

## Scalability Considerations

The current implementation prioritizes clarity and determinism. It:

Streams the input file line-by-line.
Materializes parsed entries into memory for aggregation.
Uses GroupBy + sorting to determine the top 3 URLs and IP addresses.

This results in:
Time complexity: O(N + U log U + I log I)
Space complexity: O(N)

Where:
N = number of log entries
U = number of unique URLs
I = number of unique IP addresses

For very large log files (e.g. millions of entries), a more memory-efficient approach could be used:

High-volume optimization approach
Instead of materializing all entries and sorting all unique keys:
Maintain Dictionary<string, int> counters for URLs and IPs while streaming.
Track the top 3 items using a fixed-size structure (e.g. a small ordered list of size 3).
Update this structure incrementally as counts change.
Avoid sorting the full set of unique keys.

This would reduce:
Memory from O(N) to O(U + I)
Sorting cost from O(U log U) to O(U)

The current implementation keeps the code straightforward while still being efficient for typical log sizes.