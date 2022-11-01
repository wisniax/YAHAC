using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SharpNBT;
using Windows.Graphics;
using System.Xml.Linq;
using System.Text.Json;

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

		public class CowlectionNbtItemTag
		{
			public string id { get; set; }
			public int Count { get; set; }
			public CowlectionTag tag { get; set; }
			public int Damage { get; set; }
		}
		public class CowlectionTag
		{
			//public int HideFlags { get; set; }
			//public SkullOwner SkullOwner { get; set; }
			//public Display display { get; set; }
			public CowlectionExtraAttributes ExtraAttributes { get; set; }
		}
		public class CowlectionExtraAttributes
		{
			//public string modifier { get; set; }
			public object attributes { get; set; }
			public string id { get; set; }
			//public string uuid { get; set; }
			//public int donated_museum { get; set; }
			//public string timestamp { get; set; }
		}

		public static CowlectionNbtItemTag ReadCowlectionNbtFromClipboard()
		{
			try
			{
				return JsonSerializer.Deserialize<CowlectionNbtItemTag>(CopyToClipboard.GetFromClipboard());
			}
			catch (Exception)
			{
				return null;
			}
		}

		/*"§bArachno X",
    "§bArachno Resistance X",
    "§bAttack Speed X",
    "§bBlazing Fortune X",
    "§bBlazing Resistance X",
    "§bBreeze X",
    "§bCombo X",
    "§bDouble Hook X",
    "§bElite X",
    "§bExperience X",
    "§bFisherman X",
    "§bFishing Experience X",
    "§bFishing Speed X",
    "§bHunter X",
    "§bIgnition X",
    "§bInfection X",
    "§bLife Recovery X",
    "§bLife Regeneration X",
    "§bLifeline X",
    "§bMana Pool X",
    "§bMidas Touch X",
    "§bSpeed X",
    "§bTrophy Hunter X",
    "§bUndead X",
    "§bUndead Resistance X",*/
	}
}
