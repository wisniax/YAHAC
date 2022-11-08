using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YAHAC.Core;

namespace YAHAC.Benchmarks
{
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn]
	public class HttpRequestersBenchmarks
	{
		private readonly HypixelApiRequester hypixelApiRequester = new(HypixelApiRequester.DataSources.Bazaar);
		private readonly HttpClient httpClient = new();

		[Benchmark(Baseline = true)]
		public async Task GetPageAsync()
		{
			var BZResult = await (await httpClient.GetAsync("https://api.hypixel.net/skyblock/bazaar")).Content.ReadAsStringAsync();
			if (BZResult == null) throw new Exception();
		}

		[Benchmark]
		public async Task GetPageWithCounterAsync()
		{
			var BZResult = (await Task.Run(async () => await hypixelApiRequester.GetBodyAsync()).Result.Content.ReadAsStringAsync());
			if (BZResult == null) throw new Exception();
		}

		//[Benchmark]
		//public void GetSourceFromComplexConv()
		//{
		//	converter.Complex_PerfectConversion(DefBitmap);
		//}
	}
}
