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
