using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAHAC.Core
{
	public class DataPatterns
	{
		public class Settings
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
	}
}
