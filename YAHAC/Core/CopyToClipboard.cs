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

		public static string GetFromClipboard()
		{
			if (Clipboard.ContainsText(TextDataFormat.Text))
			{
				string clipboardText = Clipboard.GetText(TextDataFormat.Text);
				return clipboardText;
				// Do whatever you need to do with clipboardText
			}
			else return "";
		}
	}
}
