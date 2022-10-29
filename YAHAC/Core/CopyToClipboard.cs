using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace YAHAC.Core
{
	public static class CopyToClipboard
	{
		public static void Copy(string str)
		{
			var thread = new Thread(() => Clipboard.SetText(str));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}
	}
}
