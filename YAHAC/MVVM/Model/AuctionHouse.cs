using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using YAHAC.Core;

namespace YAHAC.MVVM.Model
{
	/// <summary>
	/// All possible data in single auction
	/// The unnecessary lines are commented for memory preservation
	/// </summary>
	public class Auction
	{
		public string uuid { get; set; }
		public string auctioneer { get; set; }
		public string profile_id { get; set; }
		//public List<string> coop { get; set; }
		public long start { get; set; }
		public long end { get; set; }
		public string item_name { get; set; }
		public string item_lore { get; set; }
		public string extra { get; set; }
		public string category { get; set; }
		public string tier { get; set; }
		public int starting_bid { get; set; }
		//public string item_bytes { get; set; }
		public bool claimed { get; set; }
		//public List<object> claimed_bidders { get; set; }
		public int highest_bid_amount { get; set; }
		public long last_updated { get; set; }
		public bool bin { get; set; }
		//public List<Bid> bids { get; set; }
		public string item_uuid { get; set; }
	}

	/// <summary>
	/// All data of single bid on an item
	/// </summary>
	public class Bid
	{
		public string auction_id { get; set; }
		public string bidder { get; set; }
		public string profile_id { get; set; }
		public int amount { get; set; }
		public object timestamp { get; set; }
	}

	/// <summary>
	/// AuctionHouse 1st layer stuff
	/// </summary>
	public class AuctionHouseObj
	{
		public bool success { get; set; }
		public int page { get; set; }
		public int totalPages { get; set; }
		public int totalAuctions { get; set; }
		public long lastUpdated { get; set; }
		public List<Auction> auctions { get; set; }
	}

	public class AuctionHouse : AuctionHouseObj
	{
		BackgroundTask backgroundTask;
		HypixelApiRequester AHPageRequester;
		HypixelApiRequester AHEndedRequester;

		KeyValuePair<HttpResponseHeaders, HttpContentHeaders> latestHeaders;
		DateTimeOffset? Header_LastModified;
		TimeSpan? Header_TimeOffset;
		bool WholeAHGathered;

		public AuctionHouse()
		{
			success = false;
			WholeAHGathered = false;
			AHPageRequester = new(HypixelApiRequester.DataSources.AuctionHouse_Auctions);
			AHEndedRequester = new(HypixelApiRequester.DataSources.AuctionHouse_Ended);
		}
	}
}
