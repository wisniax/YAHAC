using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hy_ITR
{

	public static class Test
	{

		[DllImport(DllName.str, EntryPoint = "helloworld")]
		internal static extern IntPtr _helloworld();
		public static string HelloWorld()
		{
			return Marshal.PtrToStringAnsi(_helloworld())!;
		}


	}

}