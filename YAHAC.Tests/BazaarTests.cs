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
		[Fact]
		public void PopulateBazaar_Test()
		{
			//Arrange
			var sut = new Bazaar(false);
			//Act
			var result = sut.success;
			//Assert
			result.Should().BeTrue();
		}
		[Fact]
		public void TestBazaarAge()
		{
			var sut = new Bazaar(false);
			var result = sut.lastUpdated;
			(result + 60000 > DateTimeOffset.Now.ToUnixTimeMilliseconds()).Should().BeTrue();
		}
	}
}
