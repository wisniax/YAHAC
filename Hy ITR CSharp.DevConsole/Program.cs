using System.Runtime.InteropServices;

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
			while (Console.ReadKey().Key != ConsoleKey.Q) ;
			Console.WriteLine("\nDEVCONSOLE>Stop requested.");
			Test.StopThread();
			Console.WriteLine("\nDEVCONSOLE>Thread stop confirmed.");

			var err = Test.GetThreadError();
			if (err == string.Empty)
				Console.WriteLine($"DEVCONSOLE>No errors!");
			else
				Console.WriteLine($"DEVCONSOLE>There was error:\n{err}");

			Console.WriteLine("DEVCONSOLE>Goodbye!");
		}
	}
}