using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YAHAC.Converters;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.Tests
{
	public class ConverterTests
	{
		//https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
		[Theory]
		[InlineData(120.0, "120.0")]
		[InlineData(1000, "NaN")]
		[InlineData(998.9, "998.9")]
		[InlineData(998.9, "998,9", 1000, "ru-RU")]
		[InlineData(1234.5, "1,234.5", 10000)]
		[InlineData(1234.5, "1 234,5", 10000, "ru-RU")]
		public void NumberToString_Tests(double age, string expectedResponse, int maxValue = 1000, string cultureInfo = "en-US")
		{
			//Arrange
			var sut = new NumberToString();
			//Act
			var result = sut.Convert(age, typeof(String), maxValue, CultureInfo.GetCultureInfo(cultureInfo));
			//Assert
			result.Should().Be(expectedResponse);
		}
	}
}
