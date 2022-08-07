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
		public int MinecraftItemBox_Size { get; set; } = 34;
		public string ItemCrafts_Recipes { get; set; } = String.Empty;
		public string BetterAH_Query { get; set; } = String.Empty;
		public bool PlaySound { get; set; } = false;
		public string BetaTests { get; set; } = String.Empty;
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

		bool Load()
		{
			if (!Directory.Exists(SettingsPath))
			{
				return false;
			}
			StreamReader sr = new(SettingsPath + @"\user.config");
			string str = sr.ReadToEnd();
			sr.Close();
			Default = JsonSerializer.Deserialize<SettingsVars>(str);
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
			if (!File.Exists(SettingsPath + @"\user.config"))
			{
				Default = new SettingsVars();
				Save();
			}
			return false;
		}
	}
}
