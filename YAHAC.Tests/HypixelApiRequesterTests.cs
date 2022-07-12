using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using YAHAC.Core;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.Tests
{
	public class HypixelApiRequesterTests
	{
		[Theory]
		[InlineData(HypixelApiRequester.DataSources.Bazaar)]
		[InlineData(HypixelApiRequester.DataSources.AuctionHouse_Auctions)]
		[InlineData(HypixelApiRequester.DataSources.AuctionHouse_Ended)]
		[InlineData(HypixelApiRequester.DataSources.Items)]
		public async void MakeHeadRequests_Test(HypixelApiRequester.DataSources sources)
		{
			//Arrange
			var sut = new HypixelApiRequester(sources);
			//Act
			var result = await sut.GetHeadAsync();
			//Assert
			result.IsSuccessStatusCode.Should().BeTrue();
			(HypixelApiRequester.HeaderRequestsInLastMinute >= 1).Should().BeTrue();
		}

		[Theory]
		[InlineData(HypixelApiRequester.DataSources.Bazaar)]
		[InlineData(HypixelApiRequester.DataSources.AuctionHouse_Auctions)]
		[InlineData(HypixelApiRequester.DataSources.AuctionHouse_Ended)]
		[InlineData(HypixelApiRequester.DataSources.Items)]
		public async void MakeBodyRequests_Test(HypixelApiRequester.DataSources sources)
		{
			//Arrange
			var sut = new HypixelApiRequester(sources);
			//Act
			var result = await sut.GetBodyAsync();
			//Assert
			result.IsSuccessStatusCode.Should().BeTrue();
			(HypixelApiRequester.ApiRequestsInLastMinute >= 1).Should().BeTrue();
		}
	}
}
