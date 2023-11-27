using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hy_ITR
{
	public static class Version
	{
		public static string Hy_ITR_CSharp { get => "0.0.1"; }

		[DllImport(DllName.str, EntryPoint = "getVersionInfo")]
		internal static extern IntPtr _getVersionInfo();
		public static string Hy_ITR { get => Marshal.PtrToStringAnsi(_getVersionInfo())!; }
	}
}
