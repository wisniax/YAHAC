using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using YAHAC.MVVM.Model;

namespace YAHAC.Tests
{
	public class ItemsRepositoryTests
	{
		[Fact]
		public void PopulateItemsRepo_Test()
		{
			//Arrange
			var sut = new ItemsRepository();
			//Act
			var result = sut.success;
			//Assert
			result.Should().BeTrue();
		}

		[Theory]
		[InlineData("smth", null)]
		[InlineData("WOOL:5", "Lime Wool")]
		[InlineData("DECENT_COFFEE", "Decent Coffee")]
		[InlineData("SPIRIT_MASK", "Spirit Mask")]
		public void ID_to_NAME_Test(string id, string expected)
		{
			//Arrange
			var sut = new ItemsRepository();
			//Act
			var result = sut.ID_to_NAME(id);
			//Assert
			result.Should().Be(expected);
		}

		[Theory]
		[InlineData("smth", null)]
		[InlineData("WOOL:5", "Lime Wool")]
		[InlineData("DECENT_COFFEE", "Decent Coffee")]
		[InlineData("SPIRIT_MASK", "Spirit Mask")]
		public void ID_to_ITEM_Test(string id, string expected)
		{
			//Arrange
			var sut = new ItemsRepository();
			//Act
			var result = sut.ID_to_ITEM(id)?.name;
			//Assert
			result.Should().Be(expected);
		}
	}
}
