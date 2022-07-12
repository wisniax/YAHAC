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
	public class Settings
	{
		static string _settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YAHAC";
		private DataPatterns.Settings _Default;

		public DataPatterns.Settings Default
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
			Default = JsonSerializer.Deserialize<DataPatterns.Settings>(str);
			return true;

		}
		public bool Save()
		{
			if (!Directory.Exists(SettingsPath))
			{
				return false;
			}
			StreamWriter sw = new(SettingsPath + @"\user.config");
			string str = JsonSerializer.Serialize<DataPatterns.Settings>(Default);
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
				Default = new DataPatterns.Settings();
				Save();
			}
			return false;
		}
	}
}
