using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YAHAC.Core;
// ReSharper disable InconsistentNaming

namespace YAHAC.Properties
{
	//Settings
	public class SettingsVars
	{
		public UserInterfaces Starting_Ui { get; set; } = UserInterfaces.Settings;
		public System.Windows.Visibility DebugVisibility { get; set; } = System.Windows.Visibility.Hidden;
		public int MinecraftItemBox_Size { get; set; } = 66;
		public string ItemCrafts_Recipes { get; set; } = string.Empty;
		public List<ItemsToSearchForCatalogue> BetterAH_ItemsToSearchForCatalogues { get; set; } = new();
		[Obsolete("Use BetterAH_ItemsToSearchForCatalogues instead")]
		public List<ItemToSearchFor> BetterAH_Query { get; set; } = new();
		public bool PlaySound { get; set; } = false;
		public string BetaTests { get; set; } = string.Empty;
	}

	public class ItemsToSearchForCatalogue
	{
		public ItemsToSearchForCatalogue()
		{
			
		}
		public ItemsToSearchForCatalogue(string Name, List<ItemToSearchFor> ItemsToSearchFor)
		{
			this.Name = Name;
			this.ID = Name.Replace(" ", "_").ToUpper();
			this.Items = ItemsToSearchFor;
		}
		public string Name { get; set; } = string.Empty;
		public string ID { get; set; } = string.Empty;
		public List<ItemToSearchFor> Items { get; set; } = new();
	}

	public class ItemToSearchFor
	{
		public ItemToSearchFor(string item_dictKey, List<string> searchQueries = null, UInt32 maxPrice = 0,
			UInt16 priority = 0, bool enabled = true, bool playSound = false)
		{
			this.item_dictKey = item_dictKey;
			this.searchQueries = searchQueries ?? new();
			this.maxPrice = maxPrice;
			this.priority = priority;
			this.enabled = enabled;
			this.playSound = playSound;
			//this.recipe_key = recipe_key;
		}
		public ItemToSearchFor(ItemToSearchFor itemToSearchFor)
		{
			item_dictKey = itemToSearchFor.item_dictKey;
			searchQueries = itemToSearchFor.searchQueries;
			maxPrice = itemToSearchFor.maxPrice;
			priority = itemToSearchFor.priority;
			enabled = itemToSearchFor.enabled;
			playSound = itemToSearchFor.playSound;
		}
		public ItemToSearchFor() { }
		public string item_dictKey { get; set; }
		public List<String> searchQueries { get; set; }
		public UInt32 maxPrice { get; set; }
		public UInt16 priority { get; set; }
		public string recipe_key { get; set; }
		public bool enabled { get; set; }
		public bool playSound { get; set; }
	}

	public enum UserInterfaces
	{
		Bazaar,
		AuctionHouse,
		BetterAH,
		Settings
	}


	public class Settings
	{
		public SettingsVars Default { get; set; }

		public static string SettingsPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YAHAC";

		public Settings()
		{
			if (!Load()) createNew();
		}

		public bool Load()
		{
			if (!Directory.Exists(SettingsPath))
			{
				return false;
			}
			try
			{
				StreamReader sr = new(SettingsPath + @"\user.json");
				string str = sr.ReadToEnd();
				sr.Close();
				Default = JsonSerializer.Deserialize<SettingsVars>(str);
			}
			catch (Exception e)
			{
				if (e is JsonException or FileNotFoundException) return false;
				throw;
			}
			return true;

		}
		public bool Save()
		{
			if (!Directory.Exists(SettingsPath))
			{
				return false;
			}
			StreamWriter sw = new(SettingsPath + @"\user.json");
			string str = JsonSerializer.Serialize<SettingsVars>(Default);
			sw.Write(str);
			sw.Flush();
			sw.Close();
			return true;
		}

		private bool createNew()
		{
			if (!Directory.Exists(SettingsPath)) Directory.CreateDirectory(SettingsPath);
			try
			{
				//Migration to new ConfigFile
				StreamReader sr = new(SettingsPath + @"\user.config");
				string str = sr.ReadToEnd();
				sr.Close();
				Default = JsonSerializer.Deserialize<SettingsVars>(str);
				Default.BetterAH_ItemsToSearchForCatalogues = ConvertOldItemsToSearchForConfig();
				Default.BetterAH_Query = new();
				Save();
			}
			catch (Exception e)
			{
				if (e is not JsonException and not FileNotFoundException) throw;
				Default = new SettingsVars();
				Default.BetterAH_Query = new();
				Default.BetterAH_ItemsToSearchForCatalogues = new();
				Save();
			}
			return true;
		}

		private List<ItemsToSearchForCatalogue> ConvertOldItemsToSearchForConfig()
		{
			List<ItemsToSearchForCatalogue> itemsToSearchForCatalogues = new();
			ItemsToSearchForCatalogue itemsToSearchForCatalogue = new("Default", new List<ItemToSearchFor>());
			itemsToSearchForCatalogue.Items.AddRange(Default.BetterAH_Query);
			itemsToSearchForCatalogues.Add(itemsToSearchForCatalogue);
			return itemsToSearchForCatalogues;
		}
	}
}
