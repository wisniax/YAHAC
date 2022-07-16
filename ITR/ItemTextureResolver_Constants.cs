using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITR
{
	public partial class ItemTextureResolver
	{
		private struct HyItems
		{

			public bool success { get; set; }
			public UInt64 lastUpdated { get; set; }
			public List<HyItems_Item> items { get; set; }
		}

		private struct HyItems_Item
		{
			public string id { get; set; }
			public string material { get; set; }
			public int durability { get; set; }
			public bool glowing { get; set; }
			public string name { get; set; }
			public string tier { get; set; }
			public string color { get; set; }
			public string skin { get; set; }
		}

		private struct Cit_Item
		{
			public string HyPixel_ID { get; set; }
			public string Name_pattern { get; set; }
			public Image Texture { get; set; }
		}

		private struct VanillaID
		{
			public int type { get; set; }
			public int meta { get; set; }
			public string name { get; set; }
			public string text_type { get; set; }
		}

	}
}
