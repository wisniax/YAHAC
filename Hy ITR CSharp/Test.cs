using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hy_ITR
{

	public static class Test
	{
		[DllImport(DllName.str, EntryPoint = "newBez")]
		internal static extern IntPtr _newBez(ulong size);
		[DllImport(DllName.str, EntryPoint = "getBezSize")]
		internal static extern ulong _getBezSize(IntPtr bez);
		[DllImport(DllName.str, EntryPoint = "deleteBez")]
		internal static extern void _deleteBez(IntPtr bez);

		/// <summary>
		/// Function that converts dynamic cstr to c# string. Frees unmanaged memory after read.
		/// </summary>
		/// <param name="strPtr"></param>
		/// <returns></returns>
		internal static string Baz2Str(IntPtr baz)
		{
			string ret = Marshal.PtrToStringAnsi(baz)!;
			_deleteBez(baz);
			return ret;
		}

		internal static IntPtr Str2Baz(string str)
		{
			byte[] str8 = Encoding.UTF8.GetBytes(str, 0, str.Length);
			byte[] str8null = new byte[str8.Length+1];
			str8null[str8.Length] = 0; //such wow
			Array.Copy(str8, str8null, str8.Length);

			var baz = _newBez((ulong)str8.Length+1);

			Marshal.Copy(str8null, 0, baz, str8null.Length);

			return baz;
		}


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

		[DllImport(DllName.str, EntryPoint = "isThreadRunning")]
		internal static extern bool _isThreadRunning();
		/// <summary>
		/// Temp function
		/// </summary>
		public static bool IsThreadRunning() => _isThreadRunning();

		[DllImport(DllName.str, EntryPoint = "getThreadError")]
		internal static extern IntPtr _getThreadError();
		/// <summary>
		/// Temp function
		/// </summary>
		public static string GetThreadError()
		{
			string err = Baz2Str(_getThreadError());
			if (err == null)
				return "no errors";
			return err;
		}
	}
}