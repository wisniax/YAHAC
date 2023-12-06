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
			Console.WriteLine("\nDEVCONSOLE> PRESS Q TO SEND HALT!\n");

			Test.SpawnThread();
			Task KeyPressTask = Task.Run(() => { while (Console.ReadKey(true).Key != ConsoleKey.Q) ; Console.WriteLine("DEVCONSOLE>Thread stop requested by user!"); });
			Task DetectStopTask = Task.Run(async () => { while (Hy_ITR.Test.IsThreadRunning()) await Task.Delay(1000); Console.WriteLine("DEVCONSOLE>Thread stop detected!"); });

			if(Task.WaitAny(KeyPressTask, DetectStopTask) == 0) 
				Test.StopThread();
			
			Console.WriteLine("DEVCONSOLE>Thread stop confirmed.");

			var err = Test.GetThreadError();
			if (err == string.Empty)
				Console.WriteLine($"DEVCONSOLE>No errors!");
			else
				Console.WriteLine($"DEVCONSOLE>There was error:\n{err}");

			Console.WriteLine("DEVCONSOLE>Goodbye!");
		}
	}
}