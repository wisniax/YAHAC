using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;
using YAHAC.Converters;
using BenchmarkDotNet.Order;

namespace YAHAC.Benchmarks
{
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn]
	public class BitmapToWPFSourceBenchmarks
	{
		private Bitmap DefBitmap = YAHAC.Properties.Resources.NoTextureMark;
		private static readonly BitmapToWPFSource converter = new();

		[Benchmark(Baseline = true)]
		public void GetSourceFromBasicConv()
		{
#pragma warning disable CS0612 // Type or member is obsolete
			converter.Basic_NoAlpha(DefBitmap);
#pragma warning restore CS0612 // Type or member is obsolete
		}

		[Benchmark]
		public void GetSourceFromComplexConv()
		{
			converter.Complex_PerfectConversion(DefBitmap);
		}

	}
}
