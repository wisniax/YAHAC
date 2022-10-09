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
					result.Should().BeNull();
					break;
				case false:
					result.Should().NotBeNull();
					break;
			}
		}

		//This code ought to be relative broo

		//[Theory]
		//[InlineData("smth", null)]
		//[InlineData("WOOL:5", "Vanilla")]
		//[InlineData("DECENT_COFFEE", "HyPixelSkull")]
		//[InlineData("SPIRIT_MASK", "HyPixelSkull")]
		//public void ID_to_RESOURCEPACKNAME_Test(string id, string? shouldBe)
		//{
		//	//Arrange
		//	var sut = new ItemTextureResolver();
		//	sut.FastInit(noFileCache: true);
		//	var loaded = sut.LoadResourcepack(@"C:\Users\marci\source\repos\YAHAC\YAHAC.Tests\bin\Debug\net6.0-windows10.0.22000.0\Worlds_and_Beyond_1_4_1.zip");
		//	//Act
		//	var result = sut.GetItemFromID(id)?.ResourcePackName;
		//	Task.Delay(5000).Wait();
		//	result = sut.GetItemFromID(id)?.ResourcePackName;
		//	//Assert

		//	result.Should().Be(shouldBe);
		//}

		[Theory]
		[InlineData("", Rarity.Common)]
		[InlineData(null, Rarity.Common)]
		[InlineData("UNCOMMON", Rarity.Uncommon)]
		[InlineData("sdfhjv", Rarity.Custom)]
		[InlineData("VERY_SPECIAL", Rarity.Very_Special)]
		public void String_to_RarityEnum_Test(string str, Rarity expected)
		{
			//Arrange
			//Act
			var result = Item.GetRarityFromString(str);
			//Assert
			result.Should().Be(expected);
		}
	}
}
