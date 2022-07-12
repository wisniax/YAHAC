using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using YAHAC.Core;
using YAHAC.MVVM.Model;

namespace YAHAC.Tests
{
	public class BazaarTests
	{
		[Theory]
		[InlineData(false)]
		public void PopulateBazaar_Test(bool KeepUpdated)
		{
			//Arrange
			var sut = new Bazaar(KeepUpdated);
			//Act
			var result = sut.success;
			//Assert
			result.Should().BeTrue();
		}
	}
}
