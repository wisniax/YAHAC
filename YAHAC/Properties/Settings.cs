using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YAHAC.Properties
{
	public static class Settings
	{
		static string _settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YAHAC";

		public static SettingsStruct Default { get; set; }

		public static string SettingsPath
		{
			get { return _settingsPath; }
		}

		static Settings()
		{
			if (!Load()) createNew();
		}

		static bool Load()
		{
			if (!Directory.Exists(SettingsPath))
			{
				return false;
			}
			StreamReader sr = new(SettingsPath + @"\user.config");
			string str = sr.ReadToEnd();
			sr.Close();
			Default = JsonSerializer.Deserialize<SettingsStruct>(str);
			return true;

		}
		public static bool Save()
		{
			if (!Directory.Exists(SettingsPath))
			{
				return false;
			}
			StreamWriter sw = new(SettingsPath + @"\user.config");
			string str = JsonSerializer.Serialize<SettingsStruct>(Default);
			sw.Write(str);
			sw.Flush();
			sw.Close();
			return true;
		}
		static bool createNew()
		{
			if (!Directory.Exists(SettingsPath))
			{
				Directory.CreateDirectory(SettingsPath);
			}
			if (!File.Exists(SettingsPath + @"\user.config"))
			{
				Default = new SettingsStruct();
				Save();
			}
			return false;
		}

		public class SettingsStruct
		{
			public UserInterfaces Starting_Ui { get; set; } = UserInterfaces.Settings;
			public int MinecraftItemBox_Size { get; set; } = 34;
			public string ItemCrafts_Recipes { get; set; } = String.Empty;
			public string BetterAH_Query { get; set; } = String.Empty;
			public bool PlaySound { get; set; } = false;
			public string BetaTests { get; set; } = String.Empty;

			public bool Save()
			{
				return Settings.Save();
			}
		}
		public enum UserInterfaces
		{
			Bazaar,
			AuctionHouse,
			Settings,
			BetterAH
		}
	}
}
