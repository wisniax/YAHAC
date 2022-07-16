using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITR
{
	/// <summary>
	/// This class contains most needed info about Item
	/// </summary>
	[System.Runtime.Versioning.SupportedOSPlatform(platformName: "windows")]
	public class Item
	{
		/// <summary>
		/// Item in-game name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// item ID used by HyPixel SkyBlock
		/// </summary>
		public string HyPixel_ID { get; }

		/// <summary>
		/// Bukkit Material of Item
		/// </summary>
		public Material Material { get; }

		/// <summary>
		/// Meta ID of Item
		/// </summary>
		public int Meta_ID { get; }

		/// <summary>
		/// GIF or PNG Image
		/// </summary>
		public Image Texture { get; }

		/// <summary>
		/// Is this texture of item original from HyPixel_ID? (or material texture)
		/// </summary>
		public bool IsOriginalTexture { get; }

		/// <summary>
		/// Has enchanted effect?
		/// </summary>
		public bool Glow { get; }

		/// <param name="name">Item in-game name</param>
		/// <param name="hyPixel_ID">item ID used by HyPixel SkyBlock</param>
		/// <param name="material">Bukkit Material of Item</param>
		/// <param name="originalTexture">Is this texture of item original from HyPixel_ID? (or material texture)</param>
		/// <param name="texture">GIF or PNG Image</param>
		/// <param name="glow">Has enchanted effect?</param>
		/// <param name="meta_ID">Meta ID</param>
		public Item(string name, string hyPixel_ID, Material material, bool originalTexture, Image texture, bool glow, int meta_ID = 0)
		{
			Name = name;
			HyPixel_ID = hyPixel_ID;
			Material = material;
			Meta_ID = meta_ID;
			IsOriginalTexture = originalTexture;
			Texture = texture;
			Glow = glow;
		}
	}

}
