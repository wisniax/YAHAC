// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using System;
namespace YAHAC.Benchmarks
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run<BitmapToWPFSourceBenchmarks>();
		}
	}
}
	