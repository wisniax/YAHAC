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
    public class AuctionHouseTests
    {
        [Fact]
        void PopulateAH_Test()
        {
            var sut = new AuctionHouse();
            sut.success.Should().BeTrue();
        }
    }
}
