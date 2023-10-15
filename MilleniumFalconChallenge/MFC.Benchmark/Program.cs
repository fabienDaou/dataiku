using BenchmarkDotNet.Running;
using MFC.Benchmark;

var summary = BenchmarkRunner.Run<RunnerTests>();
