namespace Hy_ITR.DevConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(string.Concat(Enumerable.Repeat("=", 50)));
			Console.WriteLine($"   Hy ITR CSharp v{Version.Hy_ITR_CSharp}   Hy ITR v{Version.Hy_ITR}");
			Console.WriteLine(string.Concat(Enumerable.Repeat("=", 50)));

			Console.WriteLine($"DEVCONSOLE>Hello World!");
			Console.WriteLine($"DEVCONSOLE>{Test.HelloWorld()}");
			Test.SpawnThread();
			System.Threading.Thread.Sleep(2000);
			Console.WriteLine("\nDEVCONSOLE>Stop requested.");
			Test.StopThread();
			Console.WriteLine("\nDEVCONSOLE>Thread stop confirmed.");
			Console.WriteLine($"DEVCONSOLE> errors: \"{Test.GetThreadError()}\"");
			Console.WriteLine("DEVCONSOLE>Goodbye!");
		}
	}
}