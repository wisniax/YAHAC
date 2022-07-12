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
		public class BazaarObj
		{
			public bool success { get; set; }
			public long lastUpdated { get; set; }
			public Dictionary<string, BazaarItemDef> products { get; set; }
		}
		public class BazaarItemDef
		{
			public string product_name { get; set; } //Translation from prod_id to item name requ
			public string product_id { get; set; }
			public List<BzOrders> sell_summary { get; set; }
			public List<BzOrders> buy_summary { get; set; }
			public Quick_status quick_status { get; set; }
		}
		public class BzOrders
		{
			public UInt32 amount { get; set; }
			public decimal pricePerUnit { get; set; }
			public UInt16 orders { get; set; }

		}
		public class Quick_status
		{
			public string productId { get; set; }
			public double sellPrice { get; set; }
			public UInt32 sellVolume { get; set; }
			public UInt32 sellMovingWeek { get; set; }
			public UInt16 sellOrders { get; set; }
			public double buyPrice { get; set; }
			public UInt32 buyVolume { get; set; }
			public UInt32 buyMovingWeek { get; set; }
			public UInt16 buyOrders { get; set; }
		}
	}
}
