// Run all benchmarks: dotnet run -c Release --project benchmarks/SharpMath.Benchmarks -- --filter "*"
// See doc/benchmarks.md for results and the comparison with SharpMath 1.x.

using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
