using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hy_ITR
{

	public static class Test
	{

		[DllImport(DllName.str, EntryPoint = "helloworld")]
		internal static extern IntPtr _helloworld();
		/// <summary>
		/// Simple Hello World for testing.
		/// </summary>
		/// <returns></returns>
		public static string HelloWorld()
		{
			return Marshal.PtrToStringAnsi(_helloworld())!;
		}

		[DllImport(DllName.str, EntryPoint = "spawnThread")]
		internal static extern void _spawnThread();
		/// <summary>
		/// Temp function
		/// </summary>
		public static void SpawnThread() => _spawnThread();

		[DllImport(DllName.str, EntryPoint = "stopThread")]
		internal static extern void _stopThread();
		/// <summary>
		/// Temp function
		/// </summary>
		public static void StopThread() => _stopThread();
	}

}