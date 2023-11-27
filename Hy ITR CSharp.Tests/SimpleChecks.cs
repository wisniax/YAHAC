namespace Hy_ITR.Tests
{
	public class SimpleChecks
	{
		[Fact]
		public void VersionTest()
		{
			Assert.Equal(Version.Hy_ITR_CSharp, Version.Hy_ITR);
		}

		[Fact]
		public void HelloWorldTest()
		{
			Assert.Equal("Hello World, from Hy ITR!", Test.HelloWorld());
		}
	}
}