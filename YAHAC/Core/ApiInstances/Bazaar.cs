//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace YAHAC.Core.ApiInstances
//{
//	internal class Bazaar
//	{
//	}

//	[Obsolete]
//	public static class BazaarCheckup
//	{
//		static private HttpCliento httpCliento;
//		static private string bzString;
//		static private string bzUrl = "https://api.hypixel.net/skyblock/bazaar";
//		static public BazaarObj bazaarObj;

//		//constructor 
//		static BazaarCheckup()
//		{
//			httpCliento = new HttpCliento();
//			bazaarObj = new BazaarObj();
//			bazaarObj.age = 0;
//		}
//		static public void refresh()
//		{
//			if ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - BazaarCheckup.bazaarObj.age < 12500) && DateTimeOffset.Now.ToUnixTimeMilliseconds() - bazaarObj.lastUpdated < 25000) return;
//			var bzTask = httpCliento.GetAsync(bzUrl);
//			var cachedBz = bzTask.Result.Content.ReadAsStringAsync();
//			bzString = cachedBz.Result;
//			BazaarObj bazaarObjtemp = new BazaarObj();
//			var age = DateTimeOffset.Now.ToUnixTimeMilliseconds();
//			bazaarObjtemp = deserializeBz(bzString);
//			bazaarObjtemp.age = age;
//			if (Properties.AllItemsREPO.itemRepo.success != true) return;
//			foreach (var item in bazaarObjtemp.products)
//			{
//				item.Value.product_name = Properties.AllItemsREPO.IDtoNAME(item.Value.product_id);
//			}
//			if (bazaarObjtemp.products.ContainsKey("BAZAAR_COOKIE")) { bazaarObjtemp.products.Remove("BAZAAR_COOKIE"); }
//			bazaarObj = bazaarObjtemp;
//		}
//		static private BazaarObj deserializeBz(string toDes)
//		{ //https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0
//			BazaarObj bazaarObj = JsonSerializer.Deserialize<BazaarObj>(toDes);
//			return bazaarObj;
//		}

//		public struct BazaarObj
//		{
//			public bool success { get; set; }
//			public long lastUpdated { get; set; }
//			public long age { get; set; }
//			public Dictionary<string, BazaarItemDef> products { get; set; }
//		}
//		public class BazaarItemDef
//		{
//			public string product_name { get; set; } //Translation from prod_id to item name requ
//			public string product_id { get; set; }
//			public List<BzOrders> sell_summary { get; set; }
//			public List<BzOrders> buy_summary { get; set; }
//			public Quick_status quick_status { get; set; }
//		}
//		public struct BzOrders
//		{
//			public UInt32 amount { get; set; }
//			public decimal pricePerUnit { get; set; }
//			public UInt16 orders { get; set; }

//		}
//		public struct Quick_status
//		{
//			public string productId { get; set; }
//			public double sellPrice { get; set; }
//			public UInt32 sellVolume { get; set; }
//			public UInt32 sellMovingWeek { get; set; }
//			public UInt16 sellOrders { get; set; }
//			public double buyPrice { get; set; }
//			public UInt32 buyVolume { get; set; }
//			public UInt32 buyMovingWeek { get; set; }
//			public UInt16 buyOrders { get; set; }
//		}
//	}

//	[Obsolete]
//	//Reference: https://docs.microsoft.com/pl-pl/dotnet/api/system.net.http.httpclient?view=net-6.0
//	class HttpCliento : HttpClient
//	{
//		public static int reqInLastMinute;
//		public static List<long> handledRequests = new List<long>();
//		new public Task<HttpResponseMessage> GetAsync(string strong)
//		{
//			handledRequests.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
//			handledRequests = handledRequests.Where((long x) => DateTimeOffset.Now.ToUnixTimeMilliseconds() - x < 60000).ToList();
//			reqInLastMinute = handledRequests.Count;
//			return base.GetAsync(strong);
//		}
//	}
//}
