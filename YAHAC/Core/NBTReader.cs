using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAHAC.Core
{
	public class NBTReader
	{
		public static string ReadNBTFromB64String(string str)
		{
			str = str.Replace(@"\u003d", "="); //Must have bc HYPIXEL :)
			var byteArray = Convert.FromBase64String(str);
			MemoryStream memoryStream = new(byteArray);
			GZipStream gZipStream = new(memoryStream, CompressionMode.Decompress, false);
			SharpNBT.TagReader tagReader = new(gZipStream, SharpNBT.FormatOptions.BigEndian);
			SharpNBT.TagContainer tag = tagReader.ReadTag() as SharpNBT.TagContainer;
			return tag.Stringify();
		}
	}
}
