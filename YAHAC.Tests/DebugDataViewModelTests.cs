using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.Tests
{
	public class DebugDataViewModelTests
	{
		[Theory]
		[InlineData("120.0","120.0")]
		[InlineData("999.9","NaN")]
		[InlineData("998.9", "998.9")]
		[InlineData(null, "NaN")]
		public void DataFormatting_BazaarAge_Test(string age, string expectedResponse)
		{
			//Arrange
			var sut = new DebugDataViewModel(false);
			sut.BazaarAge = age;
			//Act
			var result = sut.BazaarAge;
			//Assert
			result.Should().Be(expectedResponse);
		}

		[Theory]
		[InlineData("120.0", "120.0")]
		[InlineData("999.9", "NaN")]
		[InlineData("998.9", "998.9")]
		[InlineData(null, "NaN")]
		public void DataFormatting_AuctionHouse_Test(string age, string expectedResponse)
		{
			//Arrange
			var sut = new DebugDataViewModel(false);
			sut.AuctionHouseAge = age;
			//Act
			var result = sut.AuctionHouseAge;
			//Assert
			result.Should().Be(expectedResponse);
		}
	}
}
