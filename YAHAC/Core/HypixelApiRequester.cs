using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace YAHAC.Core
{
	public class HypixelApiRequester : HttpClient
	{
		public static int HeaderRequestsInLastMinute { get; set; } = 0;
		public static int ApiRequestsInLastMinute { get; set; } = 0;
		public static double UsedDataInLastMinute_InMB { get; set; } = 0;
		static List<long> HeaderRequests_HandledRequests = new List<long>();
		static List<long> ApiRequests_HandledRequests = new List<long>();

		DataSources source;

		public HypixelApiRequester(DataSources source)
		{
			this.source = source;
		}

		public Task<HttpResponseMessage> GetHeadAsync(int page = 0)
		{
			HeaderRequests_HandledRequests.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
			UpdateStatistics();
			return SendAsync(new HttpRequestMessage(HttpMethod.Head, GetUrlFromDataSource(source, page)));
		}

		public Task<HttpResponseMessage> GetBodyAsync(int page = 0)
		{
			ApiRequests_HandledRequests.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
			UpdateStatistics();
			return base.GetAsync(GetUrlFromDataSource(source, page));
		}
		void UpdateStatistics()
		{
			HeaderRequests_HandledRequests = HeaderRequests_HandledRequests.Where((long x) => DateTimeOffset.Now.ToUnixTimeMilliseconds() - x < 60000).ToList();
			HeaderRequestsInLastMinute = HeaderRequests_HandledRequests.Count();
			ApiRequests_HandledRequests = ApiRequests_HandledRequests.Where((long x) => DateTimeOffset.Now.ToUnixTimeMilliseconds() - x < 60000).ToList();
			ApiRequestsInLastMinute = ApiRequests_HandledRequests.Count();
		}
		private string GetUrlFromDataSource(DataSources source, int page = 0)
		{
			switch (source)
			{
				case DataSources.AuctionHouse_Auctions:
					return "https://api.hypixel.net/skyblock/auctions?page=" + page.ToString();
				case DataSources.AuctionHouse_Ended:
					return "https://api.hypixel.net/skyblock/auctions_ended";
				case DataSources.Bazaar:
					return "https://api.hypixel.net/skyblock/bazaar";
				case DataSources.Items:
					return "https://api.hypixel.net/resources/skyblock/items";
				default:
					return null;
			}
		}
		public enum DataSources
		{
			AuctionHouse_Auctions,
			AuctionHouse_Ended,
			Bazaar,
			Items
		}
	}

}
