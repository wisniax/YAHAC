using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinecraftTextBlock_for_dotNET_Core
{
	public static class Formatting
	{
		public static readonly Regex MinecraftFormattings = new Regex("§([0-9a-frlomnk])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}
}
