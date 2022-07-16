using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using YAHAC.MVVM.Model;
using ITR;

namespace YAHAC.Tests
{
	public class ITRTests
	{
		[Fact]
		public void PopulateItemsRepo_Test()
		{
			//Arrange
			var sut = new ItemTextureResolver();
			sut.FastInit(noFileCache: true);
			//Act
			var result = sut.Initialized;
			//Assert
			result.Should().BeTrue();
		}

		[Theory]
		[InlineData("smth", null)]
		[InlineData("WOOL:5", "Lime Wool")]
		[InlineData("DECENT_COFFEE", "Decent Coffee")]
		[InlineData("SPIRIT_MASK", "Spirit Mask")]
		public void ID_to_ITEM_Test(string id, string expected)
		{
			//Arrange
			var sut = new ItemTextureResolver();
			sut.FastInit(noFileCache: true);
			//Act
			var result = sut.GetItemFromID(id)?.Name;
			//Assert
			result.Should().Be(expected);
		}

		[Theory]
		[InlineData("smth", true)]
		[InlineData("WOOL:5", false)]
		[InlineData("DECENT_COFFEE", false)]
		[InlineData("SPIRIT_MASK", false)]
		public void ID_to_TEXTURE_Test(string id, bool shouldBeNull)
		{
			//Arrange
			var sut = new ItemTextureResolver();
			sut.FastInit(noFileCache: true);
			//Act
			var result = sut.GetItemFromID(id)?.Texture;
			//Assert
			switch (shouldBeNull)
			{
				case true:
					result.Should().Be(null);
					break;
				case false:
					result.Should().NotBe(null);
					break;
			}
		}
	}
}
