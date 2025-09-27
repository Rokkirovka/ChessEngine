```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley)
12th Gen Intel Core i5-12450H 2.00GHz, 1 CPU, 12 logical and 8 physical cores
.NET SDK 8.0.406
  [Host]     : .NET 8.0.13 (8.0.1325.6609), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.13 (8.0.1325.6609), X64 RyuJIT AVX2


```
| Method             | Mean     | Error   | StdDev  | Gen0       | Gen1     | Allocated |
|------------------- |---------:|--------:|--------:|-----------:|---------:|----------:|
| FirstMoveBenchmark | 134.3 ms | 0.85 ms | 0.71 ms | 66000.0000 | 250.0000 |    396 MB |
