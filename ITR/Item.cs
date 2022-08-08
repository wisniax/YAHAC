using System;
using System.Collections.Generic;
using System.Drawing;
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
		public System.IO.MemoryStream Texture { get; }

		/// <summary>
		/// Is this texture of item original from HyPixel_ID? (or material texture)
		/// </summary>
		public bool IsOriginalTexture { get; }

		/// <summary>
		/// Has enchanted effect?
		/// </summary>
		public bool Glow { get; }

		/// <summary>
		/// States item type eg. Helm, Boots, Axe
		/// </summary>
		public string Category { get; }

		/// <summary>
		/// Item rarity eg: Common
		/// </summary>
		public Rarity Tier { get; }

		/// <summary>
		/// Item sell price to NPC default is 0 meaning unsellable
		/// </summary>
		public double NpcSellPrice { get; }

		/// <summary>
		/// Additional flag suggesting unstackability
		/// </summary>
		public bool Unstackable { get; }

		/// <param name="name">Item in-game name</param>
		/// <param name="hyPixel_ID">item ID used by HyPixel SkyBlock</param>
		/// <param name="material">Bukkit Material of Item</param>
		/// <param name="originalTexture">Is this texture of item original from HyPixel_ID? (or material texture)</param>
		/// <param name="texture">GIF or PNG Image</param>
		/// <param name="glow">Has enchanted effect?</param>
		/// <param name="meta_ID">Meta ID</param>
		/// <param name="Category">States item type eg. Helm, Boots, Axe</param>
		/// <param name="Tier">Item rarity eg: Common</param>
		/// <param name="NpcSellPrice">Item sell price to NPC</param>
		/// <param name="Unstackable">Additional flag suggesting unstackability</param>
		public Item(string name, string hyPixel_ID, Material material, bool originalTexture, System.IO.MemoryStream texture, bool glow, int meta_ID = 0,
			string Category = null, Rarity Tier = 0, double NpcSellPrice = 0, bool Unstackable = false)
		{
			Name = name;
			HyPixel_ID = hyPixel_ID;
			Material = material;
			Meta_ID = meta_ID;
			IsOriginalTexture = originalTexture;
			Texture = texture;
			Glow = glow;
			this.Category = Category;
			this.Tier = Tier;
			this.NpcSellPrice = NpcSellPrice;
			this.Unstackable = Unstackable;
		}

		/// <param name="name">Item in-game name</param>
		/// <param name="hyPixel_ID">item ID used by HyPixel SkyBlock</param>
		/// <param name="material">Bukkit Material of Item</param>
		/// <param name="originalTexture">Is this texture of item original from HyPixel_ID? (or material texture)</param>
		/// <param name="texture">GIF or PNG Image</param>
		/// <param name="glow">Has enchanted effect?</param>
		/// <param name="meta_ID">Meta ID</param>
		/// <param name="Category">States item type eg. Helm, Boots, Axe</param>
		/// <param name="Tier">Item rarity eg: Common</param>
		/// <param name="NpcSellPrice">Item sell price to NPC</param>
		/// <param name="Unstackable">Additional flag suggesting unstackability</param>
		public Item(string name, string hyPixel_ID, Material material, bool originalTexture, System.Drawing.Image texture, bool glow, int meta_ID = 0,
			string Category = null, Rarity Tier = 0, double NpcSellPrice = 0, bool Unstackable = false)
		{
			Name = name;
			HyPixel_ID = hyPixel_ID;
			Material = material;
			Meta_ID = meta_ID;
			IsOriginalTexture = originalTexture;
			Glow = glow;
			this.Category = Category;
			this.Tier = Tier;
			this.NpcSellPrice = NpcSellPrice;
			this.Unstackable = Unstackable;

			ImageProcessor.ImageFactory imageFactory = new();
			imageFactory.Load(texture);
			imageFactory.Save(Texture);
		}



		/// <param name="item">Just ITR.Item</param>
		public Item(Item item)
		{
			Name = item.Name;
			HyPixel_ID = item.HyPixel_ID;
			Material = item.Material;
			Meta_ID = item.Meta_ID;
			IsOriginalTexture = item.IsOriginalTexture;
			Texture = item.Texture;
			Glow = item.Glow;
			Category = item.Category;
			Tier = item.Tier;
			NpcSellPrice = item.NpcSellPrice;
			Unstackable = item.Unstackable;
		}

		/// <summary>
		/// Matches string to Rarity enum
		/// </summary>
		/// <returns>Rarity enum</returns>
		public static Rarity GetRarityFromString(string str)
		{
			if (str == null) return Rarity.Common;
			if (str.Length < 2) return Rarity.Common;
			bool parsed = Enum.TryParse(str, true, out Rarity result);
			if (!parsed) return Rarity.Custom;
			return result;

		}
	}

	public enum Rarity
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Mythic,
		Divine,
		Special,
		Very_Special,
		Custom
	}

}
