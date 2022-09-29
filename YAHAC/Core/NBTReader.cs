using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace YAHAC.Core
{
	public class NBTReader
	{
		const string RegexString = "\"?\\bid\\b\"?: ?\"([A-Z_:0-9]+)\"";

		Regex regex;

		public NBTReader()
		{
			regex = new Regex(RegexString, RegexOptions.Compiled);
		}
		public string ReadNBTFromB64String(string str)
		{
			str = str.Replace(@"\u003d", "="); //Must have bc HYPIXEL :)
			var byteArray = Convert.FromBase64String(str);
			MemoryStream memoryStream = new(byteArray);
			GZipStream gZipStream = new(memoryStream, CompressionMode.Decompress, false);
			SharpNBT.TagReader tagReader = new(gZipStream, SharpNBT.FormatOptions.BigEndian);
			SharpNBT.TagContainer tag = tagReader.ReadTag() as SharpNBT.TagContainer;
			return tag.Stringify();
		}


		public string GetIdFromNbtString(string nbtString)
		{
			string itemId = regex.Match(nbtString).Groups[1].Value;
			return itemId;
		}

		public string GetIdFromB64String(string B64nbt)
		{
			return GetIdFromNbtString(ReadNBTFromB64String(B64nbt));
		}
	}
}
