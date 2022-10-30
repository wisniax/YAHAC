using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YAHAC.Core;

namespace YAHAC.Properties
{
	//Settings
	public class SettingsVars
	{
		public UserInterfaces Starting_Ui { get; set; } = UserInterfaces.Settings;
		public System.Windows.Visibility DebugVisibility { get; set; } = System.Windows.Visibility.Hidden;
		public int MinecraftItemBox_Size { get; set; } = 66;
		public string ItemCrafts_Recipes { get; set; } = String.Empty;
		public List<ItemToSearchFor> BetterAH_Query { get; set; } = new();
		public bool PlaySound { get; set; } = false;
		public string BetaTests { get; set; } = String.Empty;
	}
	public class ItemToSearchFor
	{
		public ItemToSearchFor(string item_dictKey, List<String> searchQueries = null, UInt32 maxPrice = 0, UInt16 priority = 0, bool enabled = true, bool playSound = false)
		{
			this.item_dictKey = item_dictKey;
			this.searchQueries = searchQueries == null ? new() : searchQueries;
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
			recipe_key = itemToSearchFor.recipe_key;
		}
		public ItemToSearchFor()
		{

		}
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
		static string _settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YAHAC";
		private SettingsVars _Default;

		public SettingsVars Default
		{
			get { return _Default; }
			set { _Default = value; }
		}

		public static string SettingsPath
		{
			get { return _settingsPath; }
		}

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
				StreamReader sr = new(SettingsPath + @"\user.config");
				string str = sr.ReadToEnd();
				sr.Close();
				Default = JsonSerializer.Deserialize<SettingsVars>(str);
			}
			catch (Exception e)
			{
				if (e is JsonException || e is FileNotFoundException) return false;
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
			StreamWriter sw = new(SettingsPath + @"\user.config");
			string str = JsonSerializer.Serialize<SettingsVars>(Default);
			sw.Write(str);
			sw.Flush();
			sw.Close();
			return true;
		}
		bool createNew()
		{
			if (!Directory.Exists(SettingsPath))
			{
				Directory.CreateDirectory(SettingsPath);
			}
			Default = new SettingsVars();
			Default.BetterAH_Query = new();
			Save();
			return true;
		}
	}
}
