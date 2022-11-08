``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1098/21H2)
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.400
  [Host] : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2 DEBUG  [AttachedDebugger]

Toolchain=InProcessToolchain  InvocationCount=1  IterationCount=100  
LaunchCount=1  UnrollFactor=1  

```
|                   Method |     Mean |    Error |   StdDev | NbRequestSent | NbRequestReceived | Allocated |
|------------------------- |---------:|---------:|---------:|--------------:|------------------:|----------:|
| PublishAndReceiveMessage | 23.09 ms | 1.147 ms | 3.327 ms |           110 |                61 |   1.45 MB |
